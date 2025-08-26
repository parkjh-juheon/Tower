using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
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

    [Header("넉백 설정")]
    [SerializeField] public float knockbackPower = 5f;

    [Header("점프 설정")]
    [SerializeField] public float jumpForce = 7f;
    [SerializeField] private int maxJumpCount = 2;
    private int currentJumpCount = 0;

    [Header("Ground Check 설정")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("공중 공격 설정")]
    [SerializeField] private float airAttackFallSpeed = 20f; // 낙하 속도
    [SerializeField] public Vector2 airAttackBoxSize = new Vector2(1.5f, 2f); // 앞쪽 공격 범위
    [SerializeField] public float airAttackOffsetX = 1f; // 플레이어 앞쪽 거리
    private bool isAirAttacking = false;

    private float lastAttackTime = 0f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded = false;
    private bool wasGrounded = false;  // 직전 프레임 땅 체크
    private bool isRolling = false;
    private float rollTimer = 0f;
    private int facingDirection = 1;

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

        // 공중 공격 중 착지 처리
        if (isGrounded && isAirAttacking)
        {
            EndAirAttack();
        }

        if (isAirAttacking)
        {
            AirAttackHitCheck();
        }
    }

    private void CheckGrounded()
    {
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            currentJumpCount = 0;
        }
        animator.SetBool("isGrounded", isGrounded);
    }

    private void HandleMovement()
    {
        if (!canControl || isRolling) return;

        float inputX = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);

        if (inputX > 0)
        {
            facingDirection = 1;
            spriteRenderer.flipX = true;
        }
        else if (inputX < 0)
        {
            facingDirection = -1;
            spriteRenderer.flipX = false;
        }

        animator?.SetFloat("Speed", Mathf.Abs(inputX));
    }

    private void HandleJump()
    {
        if (!canControl || isRolling) return;

        if (Input.GetKeyDown(KeyCode.Space) && currentJumpCount < maxJumpCount)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            currentJumpCount++;
            animator?.SetTrigger("Jump");
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

    private int currentAttackIndex = 0;

    private void HandleAttack()
    {
        if (!canControl) return;

        if (Input.GetKeyDown(KeyCode.X) && Time.time >= lastAttackTime + attackCooldown)
        {
            float speedRatio = baseAttackCooldown / attackCooldown;
            animator.speed = speedRatio;

            if (!isGrounded) // 공중 공격 시작
            {
                StartAirAttack();
            }
            else // 지상 콤보 공격
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

            lastAttackTime = Time.time;
            StartCoroutine(ResetAnimatorSpeed());
        }

        // 공중 공격 중일 때 낙하 공격 판정
        if (isAirAttacking)
        {
            PerformAirAttack();
        }
    }

    private void StartAirAttack()
    {
        isAirAttacking = true;
        canControl = false;
        animator?.SetTrigger("AirAttack");

        // 빠른 낙하 시작
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
        // 플레이어가 바라보는 방향(facingDirection)에 맞춰 앞쪽 위치 계산
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
        // 지상 공격 범위 (빨간 원)
        Vector2 attackPosition = (Vector2)transform.position + Vector2.right * facingDirection * attackRange * 0.5f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition, attackRange);

        // 공중 공격 범위 (파란 박스)
        Vector2 center = (Vector2)transform.position + new Vector2(facingDirection * airAttackOffsetX, 0);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, airAttackBoxSize);
    }

}
