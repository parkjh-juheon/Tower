using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [System.Serializable]
    public class CharacterStats
    {
        // 공통 스탯
        public int maxHP;
        public int currentHP;
        public float moveSpeed;
        public float dashDistance;
        public float jumpForce;
        public int maxJumpCount;
        public float attackDamage;
        public float attackCooldown;

        // 근접 전용
        public float knockbackPower;   // 넉백
        public float meleeRange;       // 공격 범위 (원거리 스탯과 공유됨)

        // 원거리 전용
        public float bulletSize;       // 총알 크기 (근접 넉백과 공유됨)
        public float bulletLifeTime;   // 지속 시간 (근접 범위와 공유됨)
        public float bulletSpeed;      // 탄속 (근접 범위와 공유됨)

        // 스탯 변환 메서드
        public void ApplyAttackType(PlayerController.AttackType type)
        {
            if (type == PlayerController.AttackType.Melee)
            {
                // 원거리 → 근접 전환
                meleeRange += bulletLifeTime;
                meleeRange += bulletSpeed;
                knockbackPower += bulletSize;
            }
            else if (type == PlayerController.AttackType.Ranged)
            {
                // 근접 → 원거리 전환
                bulletLifeTime += meleeRange;
                bulletSpeed += meleeRange;
                bulletSize += knockbackPower;
            }
        }
    }


    public enum AttackType { Melee, Ranged }
    public AttackType attackType = AttackType.Melee;
    public CharacterStats stats;

    [Header("공격 설정")][SerializeField]
    public float attackRange = 1f; 
    [SerializeField] private float baseAttackCooldown = 0.5f; 
    [SerializeField] public float attackCooldown = 0.5f;
    [SerializeField] public float attackDamage = 1;
    [SerializeField] public float knockbackPower = 5f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("원거리 공격 설정")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("구르기 설정")]
    [SerializeField] private float rollForce = 6f;
    [SerializeField] private float rollDuration = 0.5f;

    [Header("점프 설정")]
    [SerializeField] public float jumpForce = 7f; 
    [SerializeField] private int maxJumpCount = 1; // 기본 공중 점프 가능 횟수
    private int baseMaxJumpCount; // 원래 점프 횟수 저장용
    private int currentJumpCount = 0; // 현재 공중 점프 횟수
    private int groundJumpCount = 0; // 지상 점프 횟수 (추가)

    [Header("Ground Check 설정")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    [Header("공중 공격 설정")]
    [SerializeField] private float airAttackFallSpeed = 20f;
    [SerializeField] public Vector2 airAttackBoxSize = new Vector2(1.5f, 2f);
    [SerializeField] public float airAttackOffsetX = 1f;

    private float lastAttackTime = 0f;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded = true;
    private bool wasGrounded = false;
    private bool isRolling = false;
    private bool isAirAttacking = false;
    private float rollTimer = 0f;
    private int facingDirection = 1;

    public bool canControl = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseMaxJumpCount = stats.maxJumpCount;
    }

    private void Update()
    {
        CheckGrounded();
        animator.SetBool("isFalling", !isGrounded && rb.linearVelocity.y < -0.1f);

        HandleMovement();
        HandleJump();
        HandleRoll();
        HandleAttack();

        if (isGrounded && isAirAttacking) EndAirAttack();
        if (isAirAttacking) AirAttackHitCheck();

        // 타입 전환 (예시: C 키)
        if (Input.GetKeyDown(KeyCode.C))
        {
            attackType = (attackType == AttackType.Melee) ? AttackType.Ranged : AttackType.Melee;
            Debug.Log("공격 타입 변경: " + attackType);
        }
    }

    private void CheckGrounded()
    {
        wasGrounded = isGrounded;

        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

        if (!wasGrounded && isGrounded)
        {
            currentJumpCount = 0;
            groundJumpCount = 0;
            maxJumpCount = baseMaxJumpCount;
        }

        animator.SetBool("isGrounded", isGrounded);
    }

    // ==============================
    // 기존 HandleAttack(), HandleMeleeAttack() 주석 처리
    // ==============================

    // private void HandleAttack()
    // {
    //     if (!canControl) return;
    //
    //     if (Input.GetKeyDown(KeyCode.X) && Time.time >= lastAttackTime + stats.attackCooldown)
    //     {
    //         if (attackType == AttackType.Melee) HandleMeleeAttack();
    //         else if (attackType == AttackType.Ranged) HandleRangedAttack();
    //         lastAttackTime = Time.time;
    //     }
    //
    //     if (attackType == AttackType.Melee && isAirAttacking) PerformAirAttack();
    //     if (isAirAttacking) PerformAirAttack();
    // }
    //
    // private void HandleMeleeAttack()
    // {
    //     animator.speed = 1f / stats.attackCooldown;
    //     if (!isGrounded) StartAirAttack();
    //     else
    //     {
    //         currentAttackIndex = (currentAttackIndex % 3) + 1;
    //         animator?.SetTrigger($"Attack{currentAttackIndex}");
    //
    //         Vector2 attackPosition = (Vector2)transform.position + Vector2.right * facingDirection * stats.meleeRange * 0.5f;
    //         Collider2D[] hits = Physics2D.OverlapCircleAll(attackPosition, stats.meleeRange, LayerMask.GetMask("Enemy"));
    //
    //         foreach (var hit in hits)
    //         {
    //             EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
    //             if (enemy != null) enemy.TakeDamage((int)stats.attackDamage, transform.position, stats.knockbackPower);
    //         }
    //     }
    //     StartCoroutine(ResetAnimatorSpeed());
    // }


    // ==============================
    // 예전 코드 (대체용)
    // ==============================
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
                bullet.Init((int)stats.attackDamage, stats.bulletSpeed, stats.bulletSize, stats.bulletLifeTime, facingDirection);
            }
        }
    }

    private void HandleMovement()
    {
        if (!canControl || isRolling) return;
        float inputX = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(inputX * stats.moveSpeed, rb.linearVelocity.y);

        if (inputX > 0 && facingDirection != 1)
        {
            facingDirection = 1; spriteRenderer.flipX = true; FlipFirePoint();
        }
        else if (inputX < 0 && facingDirection != -1)
        {
            facingDirection = -1; spriteRenderer.flipX = false; FlipFirePoint();
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
            firePoint.localRotation = (facingDirection == 1) ? Quaternion.identity : Quaternion.Euler(0, 180f, 0);
        }
    }

    private void HandleJump()
    {
        if (!canControl || isRolling) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //  지상 점프
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

                groundJumpCount++;
                currentJumpCount = 0;

                animator?.SetTrigger("Jump");
                Debug.Log("지상 점프");
            }
            //  공중 점프
            else if (currentJumpCount < maxJumpCount)
            {
                // 땅을 밟지 않고 공중에서 첫 점프 시작한 경우
                if (groundJumpCount == 0 && currentJumpCount == 0)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    currentJumpCount = 1;

                    // 보정: "지상에서 시작한 것처럼" 추가 점프권 1개 부여
                    maxJumpCount = baseMaxJumpCount + 1;

                    animator?.SetTrigger("Jump");
                    Debug.Log("공중에서 첫 점프 시작 → 보너스 점프권 추가");
                }
                // 일반 공중 점프
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
            if (rollTimer >= rollDuration) { isRolling = false; animator?.SetBool("Roll", false); }
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded)
        {
            isRolling = true; rollTimer = 0f;
            rb.linearVelocity = new Vector2(facingDirection * rollForce, rb.linearVelocity.y);
            animator?.SetBool("Roll", true);
        }
    }

    private void StartAirAttack()
    {
        isAirAttacking = true; canControl = false;
        animator?.SetTrigger("AirAttack");
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -airAttackFallSpeed);
    }

    private void PerformAirAttack()
    {
        Vector2 attackPosition = (Vector2)transform.position + Vector2.right * facingDirection * stats.meleeRange * 0.5f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPosition, stats.meleeRange, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null) enemy.TakeDamage((int)stats.attackDamage * 2, transform.position, stats.knockbackPower * 1.5f);
        }
    }

    private void AirAttackHitCheck()
    {
        Vector2 center = (Vector2)transform.position + new Vector2(facingDirection * airAttackOffsetX, 0);
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, airAttackBoxSize, 0f, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null) enemy.TakeDamage((int)stats.attackDamage, transform.position, stats.knockbackPower);
        }
    }

    private void EndAirAttack()
    {
        isAirAttacking = false; canControl = true;
    }

    private IEnumerator ResetAnimatorSpeed()
    {
        yield return new WaitForSeconds(0.1f);
        animator.speed = 1f;
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 attackPosition = (Vector2)transform.position + Vector2.right * facingDirection * stats.meleeRange * 0.5f;
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(attackPosition, stats.meleeRange);

        Vector2 center = (Vector2)transform.position + new Vector2(facingDirection * airAttackOffsetX, 0);
        Gizmos.color = Color.blue; Gizmos.DrawWireCube(center, airAttackBoxSize);

        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow; Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }
    }
}
