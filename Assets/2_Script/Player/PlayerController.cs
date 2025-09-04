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

    [Header("넉백 설정")]
    [SerializeField] public float knockbackPower = 5f;

    [Header("점프 설정")]
    [SerializeField] public float jumpForce = 7f;
    [SerializeField] private int maxJumpCount = 1;
    [SerializeField]private float coyoteTime = 0.5f;
    private int currentJumpCount = 0;

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
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.linearVelocity = new Vector2(facingDirection * 10f, 0f);
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

            // facingDirection 값이 1(오른쪽), -1(왼쪽) 중 하나니까
            // firePoint의 x 좌표를 방향에 맞게 고정
            localPos.x = Mathf.Abs(localPos.x) * facingDirection;

            firePoint.localPosition = localPos;
        }
    }

    private void HandleJump()
    {
        if (!canControl || isRolling) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 땅에 있으면 첫 점프
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                currentJumpCount = 1;
                animator?.SetTrigger("Jump");
                Debug.Log("첫번째 점프");
            }
            // 공중 점프
            else if (currentJumpCount >= 0 && currentJumpCount < maxJumpCount)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                currentJumpCount++;
                animator?.SetTrigger("Jump");
                Debug.Log($"{currentJumpCount}번째 점프");
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
