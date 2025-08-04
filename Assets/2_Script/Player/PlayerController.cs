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

    [Header("점프 설정")]
    [SerializeField] public float jumpForce = 7f;
    [SerializeField] private int maxJumpCount = 2;
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

    public bool canControl = true; //  제어 가능 여부

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
        if (!canControl || isRolling) return;

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

    private void HandleAttack()
    {
        if (!canControl) return;

        if (Input.GetKeyDown(KeyCode.X) && Time.time >= lastAttackTime + attackCooldown)
        {
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
                    enemy.TakeDamage((int)attackDamage, transform.position);
                }
            }

            lastAttackTime = Time.time;
            StartCoroutine(ResetAnimatorSpeed());
        }
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
    }
}
