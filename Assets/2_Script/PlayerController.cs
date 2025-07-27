using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float rollForce = 6f;

    [Header("구르기 설정")]
    [SerializeField] private float rollDuration = 0.5f;

    [Header("공격 설정")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private LayerMask enemyLayer; // 공격할 대상

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

        if (isGrounded)
            animator.SetBool("isFalling", false);
        else if (rb.linearVelocity.y < -0.1f)
            animator.SetBool("isFalling", true);
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
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isRolling)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
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
        if (Input.GetKeyDown(KeyCode.E) && Time.time >= lastAttackTime + attackCooldown)
        {
            animator?.SetTrigger("Attack");  // 애니메이션 트리거

            // 근접 공격 판정
            Vector2 attackPosition = (Vector2)transform.position + Vector2.right * facingDirection * attackRange * 0.5f;
            Collider2D[] hits = Physics2D.OverlapCircleAll(attackPosition, attackRange, enemyLayer);

            foreach (var hit in hits)
            {
                // 적 체력 컴포넌트가 있다면 데미지 적용
                EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(attackDamage);
                }
            }

            lastAttackTime = Time.time;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") && collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
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
