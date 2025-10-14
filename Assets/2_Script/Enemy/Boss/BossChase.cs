using UnityEngine;

public class BossChase : MonoBehaviour
{
    [Header("플레이어 추적")]
    public Transform player;
    public float moveSpeed = 2f;
    public float stopDistance = 3f; // 공격 사거리 안에서 정지

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

        // 보스와 플레이어 충돌 무시
        if (player != null)
        {
            Collider2D bossCol = GetComponent<Collider2D>();
            Collider2D playerCol = player.GetComponent<Collider2D>();

            if (bossCol != null && playerCol != null)
                Physics2D.IgnoreCollision(bossCol, playerCol, true);
        }
    }

    void FixedUpdate()
    {
        if (!isChasing || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // 플레이어가 멀리 있으면 이동
        if (distance > stopDistance)
        {
            MoveTowards(player.position);
            reachedAttackRange = false;
        }
        else
        {
            reachedAttackRange = true;
            StopMoving();
        }
    }

    void MoveTowards(Vector3 target)
    {
        Vector2 dir = (target - transform.position).normalized;

        // 2D 횡스크롤 게임이라면 아래 한 줄 유지
        dir.y = 0;

        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);

        // 좌우 방향 전환
        if (dir.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (dir.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
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
        StopMoving();
    }

    public void ResumeChase()
    {
        isChasing = true;
    }

    public bool IsInAttackRange()
    {
        return reachedAttackRange;
    }
}
