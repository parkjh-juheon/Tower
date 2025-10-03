using UnityEngine;

public class BossChase : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 2f;
    public float detectionRange = 10f;

    [Header("추격 멈춤 거리")]
    public float stopChaseDistance = 3f; // 플레이어와 너무 가까우면 추격 중지

    private bool isChasing = false;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // 플레이어와의 충돌 무시
        if (player != null)
        {
            Collider2D bossCol = GetComponent<Collider2D>();
            Collider2D playerCol = player.GetComponent<Collider2D>();

            if (bossCol != null && playerCol != null)
            {
                Physics2D.IgnoreCollision(bossCol, playerCol, true);
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 감지 범위 안에 있으면 쫓기 시작
        if (distanceToPlayer <= detectionRange)
            isChasing = true;

        // 추격 중, 하지만 stopChaseDistance보다 멀면 이동
        if (isChasing && distanceToPlayer > stopChaseDistance)
        {
            MoveTowards(player.position);
        }
    }

    void MoveTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position);
        direction.y = 0;
        direction = direction.normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;

        // 보스가 플레이어 방향을 바라보게
        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }
}
