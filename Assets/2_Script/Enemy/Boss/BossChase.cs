using UnityEngine;

public class BossChase : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 2f;
    public float detectionRange = 10f;

    [Header("�߰� ���� �Ÿ�")]
    public float stopChaseDistance = 3f; // �÷��̾�� �ʹ� ������ �߰� ����

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

        // �÷��̾���� �浹 ����
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

        // ���� ���� �ȿ� ������ �ѱ� ����
        if (distanceToPlayer <= detectionRange)
            isChasing = true;

        // �߰� ��, ������ stopChaseDistance���� �ָ� �̵�
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

        // ������ �÷��̾� ������ �ٶ󺸰�
        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }
}
