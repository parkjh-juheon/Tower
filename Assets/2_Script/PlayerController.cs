using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float rollForce = 6f;

    [Header("구르기 설정")]
    [SerializeField] private float rollDuration = 0.5f;

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
        // 낙하 상태 판별
        if (isGrounded)
            animator.SetBool("isFalling", false);
        else if (rb.linearVelocity.y < -0.1f)
            animator.SetBool("isFalling", true);

    }

    private void HandleMovement()
    {
        if (isRolling) return;

        float inputX = Input.GetAxisRaw("Horizontal");

        // 이동 처리
        rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);

        // 방향 전환
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

        // 애니메이션: 이동 속도
        animator?.SetFloat("Speed", Mathf.Abs(inputX));
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isRolling)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;

            animator?.SetTrigger("Jump");
            Debug.Log("점프 시도");
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 오브젝트가 "Ground" 태그를 가졌고, 아래 방향에서 충돌한 경우에만 착지로 간주
        if (collision.collider.CompareTag("Ground") && collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // "Ground" 태그를 가진 오브젝트에서 벗어났을 경우 착지 상태 해제
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

}
