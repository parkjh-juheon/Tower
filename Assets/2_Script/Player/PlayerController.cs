using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum AttackType { Melee, Ranged } // 공격 타입 정의
    public AttackType attackType = AttackType.Melee; // 기본 공격 타입은 근접

    [Header("이동 설정")]
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] private float rollForce = 6f;

    [Header("구르기 설정")]
    [SerializeField] private float rollDuration = 0.5f;

    [Header("공격 설정")]
    [SerializeField] public float attackRange = 1f;
    [SerializeField] private float baseAttackCooldown = 0.5f;
    [SerializeField] public float attackCooldown = 0.5f;
    [SerializeField] public float attackDamage = 1;
    [SerializeField] private LayerMask enemyLayer;


    [Header("원거리 공격 설정")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;   // 기본 탄속
    public float bulletSize = 1f;     // 기본 크기
    public float bulletLifeTime = 1f; // 기본 수명


    [Header("넉백 설정")]
    [SerializeField] public float knockbackPower = 5f;

    [Header("점프 설정")]
    [SerializeField] public float jumpForce = 7f;
    [SerializeField] private int maxJumpCount = 1; // 기본 공중 점프 가능 횟수
    private int baseMaxJumpCount; // 원래 점프 횟수 저장용
    [SerializeField] private float coyoteTime = 0.5f;
    private int currentJumpCount = 0;   // 현재 공중 점프 횟수
    private int groundJumpCount = 0;    // 지상 점프 횟수 (추가)


    [Header("Ground Check 설정")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    [Header("공중 공격 설정")]
    [SerializeField] private float airAttackFallSpeed = 20f;
    [SerializeField] public Vector2 airAttackBoxSize = new Vector2(1.5f, 2f);
    [SerializeField] public float airAttackOffsetX = 1f;
    private bool isAirAttacking = false;

    private float lastAttackTime = 0f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded = true;
    private bool wasGrounded = false;
    private bool isRolling = false;
    private float rollTimer = 0f;
    private int facingDirection = 1;
    private float lastGroundedTime;

    public bool canControl = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        baseMaxJumpCount = maxJumpCount; // 초기값 저장
    }


    private void Update()
    {
        CheckGrounded();
        animator.SetBool("isFalling", !isGrounded && rb.linearVelocity.y < -0.1f);

        HandleMovement();
        HandleJump();
        HandleRoll();
        HandleAttack();

        if (isGrounded && isAirAttacking)
        {
            EndAirAttack();
        }

        if (isAirAttacking)
        {
            AirAttackHitCheck();
        }

        // Ground 상태 체크 로그 출력
        Debug.Log("Ground 상태: " + isGrounded);
    }

    private void CheckGrounded()
    {
        wasGrounded = isGrounded;

        isGrounded = Physics2D.OverlapBox(
            groundCheck.position,
            groundCheckSize,
            0f,
            groundLayer
        );

        if (isGrounded)
        {
            currentJumpCount = 0;
            groundJumpCount = 0;
            maxJumpCount = baseMaxJumpCount; // 착지 시 원래 값으로 복구
            lastGroundedTime = Time.time;
        }

        animator.SetBool("isGrounded", isGrounded);
    }


    private void HandleAttack()
    {
        if (!canControl) return;

        if (Input.GetKeyDown(KeyCode.X) && Time.time >= lastAttackTime + attackCooldown)
        {
            if (attackType == AttackType.Melee)
            {
                HandleMeleeAttack();
            }
            else if (attackType == AttackType.Ranged)
            {
                HandleRangedAttack();
            }

            lastAttackTime = Time.time;
        }

        if (attackType == AttackType.Melee && isAirAttacking)
        {
            PerformAirAttack();
        }

        if (isAirAttacking)
        {
            PerformAirAttack();
        }
    }

    private int currentAttackIndex = 0;

    private void HandleMeleeAttack()
    {
        float speedRatio = baseAttackCooldown / attackCooldown;
        animator.speed = speedRatio;

        if (!isGrounded) StartAirAttack();
        else
        {
            currentAttackIndex = (currentAttackIndex % 3) + 1;
            animator?.SetTrigger($"Attack{currentAttackIndex}");

            Vector2 attackPosition = (Vector2)transform.position + Vector2.right * facingDirection * attackRange * 0.5f;
            Collider2D[] hits = Physics2D.OverlapCircleAll(attackPosition, attackRange, enemyLayer);

            foreach (var hit in hits)
            {
                EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage((int)attackDamage, transform.position, knockbackPower);
                }
            }
        }

        StartCoroutine(ResetAnimatorSpeed());
    }

    private void HandleRangedAttack()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            P_Bullet bullet = bulletObj.GetComponent<P_Bullet>();
            if (bullet != null)
            {
                bullet.Init((int)attackDamage, bulletSpeed, bulletSize, bulletLifeTime, facingDirection);
            }
        }
    }


    private void HandleMovement()
    {
        if (!canControl || isRolling) return;

        float inputX = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);

        if (inputX > 0 && facingDirection != 1)
        {
            facingDirection = 1;
            spriteRenderer.flipX = true;
            FlipFirePoint();
        }
        else if (inputX < 0 && facingDirection != -1)
        {
            facingDirection = -1;
            spriteRenderer.flipX = false;
            FlipFirePoint();
        }
        animator?.SetFloat("Speed", Mathf.Abs(inputX));
    }

    private void FlipFirePoint()
    {
        if (firePoint != null)
        {
            Vector3 localPos = firePoint.localPosition;
            localPos.x = Mathf.Abs(localPos.x) * facingDirection;
            firePoint.localPosition = localPos;

            // firePoint 회전도 방향에 맞게 반전
            firePoint.localRotation = (facingDirection == 1)
                ? Quaternion.identity
                : Quaternion.Euler(0, 180f, 0);
        }
    }

    private void HandleJump()
    {
        if (!canControl || isRolling) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 지상 점프
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

                groundJumpCount++;
                currentJumpCount = 0;

                animator?.SetTrigger("Jump");
                Debug.Log("지상 점프");
            }
            // 공중 점프
            else if (currentJumpCount < maxJumpCount)
            {
                // 지상 점프 없이 공중에서 첫 점프 시작
                if (groundJumpCount == 0 && currentJumpCount == 0)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    currentJumpCount = 1;

                    // 임시로 점프권 +1 (지상에서 시작한 것과 동일하게 맞춰줌)
                    maxJumpCount = baseMaxJumpCount + 1;

                    animator?.SetTrigger("Jump");
                    Debug.Log("공중에서 첫 점프 시작 → 보너스 점프권 추가");
                }
                else
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    currentJumpCount++;

                    animator?.SetTrigger("Jump");
                    Debug.Log($"공중 점프 {currentJumpCount}/{maxJumpCount}");
                }
            }
        }
    }

    private void HandleRoll()
    {
        if (!canControl) return;

        if (isRolling)
        {
            rollTimer += Time.deltaTime;
            if (rollTimer >= rollDuration)
            {
                isRolling = false;
                animator?.SetBool("Roll", false);
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded)
        {
            isRolling = true;
            rollTimer = 0f;
            rb.linearVelocity = new Vector2(facingDirection * rollForce, rb.linearVelocity.y);
            animator?.SetBool("Roll", true);
        }
    }

    private void StartAirAttack()
    {
        isAirAttacking = true;
        canControl = false;
        animator?.SetTrigger("AirAttack");

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -airAttackFallSpeed);
    }

    private void PerformAirAttack()
    {
        Vector2 attackPosition = (Vector2)transform.position + Vector2.right * facingDirection * attackRange * 0.5f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPosition, attackRange, enemyLayer);

        foreach (var hit in hits)
        {
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage((int)attackDamage * 2, transform.position, knockbackPower * 1.5f);
            }
        }
    }

    private void AirAttackHitCheck()
    {
        Vector2 center = (Vector2)transform.position + new Vector2(facingDirection * airAttackOffsetX, 0);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, airAttackBoxSize, 0f, enemyLayer);

        foreach (var hit in hits)
        {
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage((int)attackDamage, transform.position, knockbackPower);
            }
        }
    }

    private void EndAirAttack()
    {
        isAirAttacking = false;
        canControl = true;
    }

    private IEnumerator ResetAnimatorSpeed()
    {
        yield return new WaitForSeconds(0.1f);
        animator.speed = 1f;
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 attackPosition = (Vector2)transform.position + Vector2.right * facingDirection * attackRange * 0.5f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition, attackRange);

        Vector2 center = (Vector2)transform.position + new Vector2(facingDirection * airAttackOffsetX, 0);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, airAttackBoxSize);

        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }
    }
}
