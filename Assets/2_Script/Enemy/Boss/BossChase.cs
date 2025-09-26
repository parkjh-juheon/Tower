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
