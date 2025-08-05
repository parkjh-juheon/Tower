using UnityEngine;

public class ForceTrap : MonoBehaviour
{
    [Header("트리거 범위")]
    public float triggerRadius = 3f;

    [Header("힘 설정")]
    public float forceStrength = 10f;
    public bool isPulling = true; // true = 끌어당김, false = 밀어냄

    [Header("발동 시간 설정")]
    public float forceDuration = 0.5f; // 힘 지속 시간
    public float forceCooldown = 2f;   // 다음 발동까지 대기 시간

    private Transform player;
    private Rigidbody2D playerRb;
    private bool isActive = false;
    private float forceTimer = 0f;
    private float cooldownTimer = 0f;

    void Update()
    {
        if (player != null)
        {
            cooldownTimer += Time.deltaTime;

            float distance = Vector2.Distance(transform.position, player.position);

            // 범위 안에 있고, 쿨다운이 끝난 상태에서만 발동
            if (!isActive && cooldownTimer >= forceCooldown && distance <= triggerRadius)
            {
                StartForce();
            }

            // 힘이 적용되는 중
            if (isActive)
            {
                ApplyForce();
                forceTimer += Time.deltaTime;
                if (forceTimer >= forceDuration)
                {
                    isActive = false;
                    cooldownTimer = 0f; // 쿨다운 시작
                }
            }
        }
    }

    void StartForce()
    {
        isActive = true;
        forceTimer = 0f;
    }

    void ApplyForce()
    {
        if (playerRb == null) return;

        Vector2 direction = (player.position - transform.position).normalized;

        if (!isPulling)
            direction *= -1; // 밀어내기면 방향 반전

        playerRb.AddForce(direction * forceStrength, ForceMode2D.Force);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            playerRb = other.GetComponent<Rigidbody2D>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            playerRb = null;
            isActive = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}
