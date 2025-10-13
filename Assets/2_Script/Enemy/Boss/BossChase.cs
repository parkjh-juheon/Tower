using UnityEngine;

public class BossChase : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 2f;
    public float stopDistance = 3f; // 패턴마다 동적으로 변경됨

    private bool isChasing = false;
    private bool reachedAttackRange = false;

    private Rigidbody2D rb;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }

        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

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

        float distance = Vector2.Distance(transform.position, player.position);

        // 항상 플레이어를 향해 이동
        if (distance > stopDistance)
        {
            MoveTowards(player.position);
            reachedAttackRange = false;
        }
        else
        {
            reachedAttackRange = true;
            StopMoving(); // 공격 사거리 안이면 정지
        }
    }


    void MoveTowards(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        dir.y = 0; // 2D 횡스크롤 기준 수평 이동
        transform.position += dir * moveSpeed * Time.deltaTime;

        // 좌우 방향 전환
        if (dir.x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (dir.x < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
    }

    public void StartChase(float distanceLimit)
    {
        stopDistance = distanceLimit;
        isChasing = true;
    }

    public void StopChase()
    {
        isChasing = false;
        rb.linearVelocity = Vector2.zero;
    }

    public bool IsInAttackRange()
    {
        return reachedAttackRange;
    }
}
