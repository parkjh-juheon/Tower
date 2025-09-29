using UnityEngine;

public class BossChase : MonoBehaviour
{
    public Transform player;          // 플레이어 Transform
    public float moveSpeed = 2f;      // 이동 속도
    public float detectionRange = 10f; // 감지 거리

    private bool isChasing = false;

    void Start()
    {
        // 플레이어 자동 탐색 (없으면 태그로 찾음)
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 감지 범위 안에 있으면 쫓기 시작
        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
        }

        if (isChasing)
        {
            MoveTowards(player.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // 플레이어가 보스 위에 올라왔을 때 → 위로 + 옆으로 튕겨내기
                Vector2 knockbackDir;

                // 보스의 위치 기준, 플레이어가 어느 쪽에 있는지 계산
                if (other.transform.position.x >= transform.position.x)
                    knockbackDir = new Vector2(1f, 1f);  // 오른쪽 위로 튕김
                else
                    knockbackDir = new Vector2(-1f, 1f); // 왼쪽 위로 튕김

                float knockbackForce = 20f; // 힘의 세기 (조절 가능)
                rb.linearVelocity = Vector2.zero; // 기존 속도 초기화
                rb.AddForce(knockbackDir.normalized * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }


    void MoveTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position);

        // 2D 횡스크롤 기준: 수평(x축)만 이동
        direction.y = 0;

        // 정규화 후 이동
        direction = direction.normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;

        // 보스가 플레이어 방향을 바라보게 (좌우 뒤집기)
        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }
}
