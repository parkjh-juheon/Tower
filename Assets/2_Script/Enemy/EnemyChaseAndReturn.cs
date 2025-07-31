using UnityEngine;

public class EnemyChaseAndReturn : MonoBehaviour
{
    public Transform player;               // 플레이어 Transform
    public float moveSpeed = 2f;           // 이동 속도
    public float detectionRange = 5f;      // 감지 거리
    public float returnStopDistance = 0.1f; // 원위치에 도달한 것으로 판단할 거리

    private Vector3 startPosition;         // 처음 위치 저장
    private bool isChasing = false;

    void Start()
    {
        // 처음 위치 저장
        startPosition = transform.position;

        // 플레이어 자동 탐색 (없을 경우)
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

        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
        }
        else if (distanceToPlayer > detectionRange)
        {
            isChasing = false;
        }

        if (isChasing)
        {
            MoveTowards(player.position);
        }
        else if (Vector2.Distance(transform.position, startPosition) > returnStopDistance)
        {
            MoveTowards(startPosition);
        }
    }

    void MoveTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position);

        // 수평(X축) 방향으로만 이동하게 함
        direction.y = 0;

        // 정규화한 뒤 속도 곱해 이동
        direction = direction.normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;
    }

}
