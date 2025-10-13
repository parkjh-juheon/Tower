using UnityEngine;

public class BossChase : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 2f;
    public float stopDistance = 3f; // ���ϸ��� �������� �����

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

        // �׻� �÷��̾ ���� �̵�
        if (distance > stopDistance)
        {
            MoveTowards(player.position);
            reachedAttackRange = false;
        }
        else
        {
            reachedAttackRange = true;
            StopMoving(); // ���� ��Ÿ� ���̸� ����
        }
    }


    void MoveTowards(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        dir.y = 0; // 2D Ⱦ��ũ�� ���� ���� �̵�
        transform.position += dir * moveSpeed * Time.deltaTime;

        // �¿� ���� ��ȯ
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
