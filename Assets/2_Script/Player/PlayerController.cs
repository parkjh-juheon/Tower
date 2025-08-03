using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rollForce = 6f;

    [Header("구르기 설정")]
    [SerializeField] private float rollDuration = 0.5f;

    [Header("공격 설정")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float baseAttackCooldown = 0.5f; // 기준 쿨타임 추가
    [SerializeField] private float attackCooldown = 0.5f; // 현재 쿨타임
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private LayerMask enemyLayer; // 공격할 대상

    [Header("점프 설정")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private int maxJumpCount = 2; // << 인스펙터에서 설정
    private int currentJumpCount = 0;

    [Header("Ground Check 설정")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;


    private float lastAttackTime = 0f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded = false;
    private bool isRolling = false;
    private float rollTimer = 0f;
    private int facingDirection = 1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
        HandleRoll();
        HandleAttack();

        CheckGrounded(); // ← 새로 추가

        animator.SetBool("isFalling", !isGrounded && rb.linearVelocity.y < -0.1f);
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded)
        {
            currentJumpCount = 0;
        }
    }



    private void HandleMovement()
    {
        if (isRolling) return;

        float inputX = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);

        if (inputX > 0)
        {
            facingDirection = 1;
            spriteRenderer.flipX = false;
        }
        else if (inputX < 0)
        {
            facingDirection = -1;
            spriteRenderer.flipX = true;
        }

        animator?.SetFloat("Speed", Mathf.Abs(inputX));
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentJumpCount < maxJumpCount && !isRolling)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            currentJumpCount++;

            animator?.SetTrigger("Jump");
        }
    }

    private void HandleRoll()
    {
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
    private void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.X) && Time.time >= lastAttackTime + attackCooldown)
        {
            // 애니메이션 속도 조절
            float speedRatio = baseAttackCooldown / attackCooldown;
            animator.speed = speedRatio;

            animator?.SetTrigger("Attack");

            Vector2 attackPosition = (Vector2)transform.position + Vector2.right * facingDirection * attackRange * 0.5f;
            Collider2D[] hits = Physics2D.OverlapCircleAll(attackPosition, attackRange, enemyLayer);

            foreach (var hit in hits)
            {
                EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(attackDamage, transform.position);
                }
            }

            lastAttackTime = Time.time;

            // 일정 시간 후 애니메이션 속도 초기화
            StartCoroutine(ResetAnimatorSpeed());
        }
    }

    private IEnumerator ResetAnimatorSpeed()
    {
        yield return new WaitForSeconds(0.1f); // 공격 애니메이션 클립이 실행되도록 약간의 시간 대기
        animator.speed = 1f; // 다시 원래 속도로 복원
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") && collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
            currentJumpCount = 0; // 땅에 닿으면 점프 카운트 초기화
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }


    private void OnDrawGizmosSelected()
    {
        // 공격 범위 시각화
        Vector2 attackPosition = (Vector2)transform.position + Vector2.right * facingDirection * attackRange * 0.5f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition, attackRange);
    }
}