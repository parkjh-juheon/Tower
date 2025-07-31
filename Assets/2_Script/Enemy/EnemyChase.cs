using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public Transform player;              // 추적할 대상
    public float moveSpeed = 2f;          // 이동 속도
    public float detectionRange = 5f;     // 인식 거리

    private bool isChasing = false;

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 인식 거리 안에 플레이어가 있는지 체크
        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }

        // 추적
        if (isChasing)
        {
            ChasePlayer();
        }
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
    }
}
