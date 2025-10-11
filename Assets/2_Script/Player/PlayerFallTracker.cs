using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerFallTracker : MonoBehaviour
{
    [Header("낙하 감지 설정")]
    public float fallHeightThreshold = 5f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;

    [Header("최근 낙하 기록 설정")]
    public float highFallMemoryTime = 1f; // 다음 발판 반응 허용 시간
    public bool hasRecentHighFall { get; private set; } = false;

    private float highFallTimer = 0f;
    private Rigidbody2D rb;
    private float startFallY;
    private bool isFalling;
    private bool wasGrounded;

    // 높은 낙하 착지 시 이벤트 (착지한 콜라이더 전달)
    public event Action<Collider2D> OnHighFallLanded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        float velocityY = rb.linearVelocity.y;

        // 낙하 시작 감지
        if (!isGrounded && velocityY < -0.1f && !isFalling)
        {
            isFalling = true;
            startFallY = transform.position.y;
        }

        // 착지 감지
        if (isFalling && isGrounded && !wasGrounded)
        {
            float fallDistance = startFallY - transform.position.y;

            if (fallDistance >= fallHeightThreshold)
            {
                Debug.Log($"높은 곳에서 착지! (낙하 거리: {fallDistance:F2})");

                // 최근 높은 낙하 기록
                hasRecentHighFall = true;
                highFallTimer = highFallMemoryTime;

                // 착지 지점 발판 이벤트 호출
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);
                if (hit.collider != null)
                {
                    OnHighFallLanded?.Invoke(hit.collider);
                }
            }

            isFalling = false;
        }

        // 최근 높은 낙하 타이머 업데이트
        if (hasRecentHighFall)
        {
            highFallTimer -= Time.deltaTime;
            if (highFallTimer <= 0f)
                hasRecentHighFall = false;
        }

        wasGrounded = isGrounded;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
