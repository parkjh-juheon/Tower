using UnityEngine;

public class BossChase : MonoBehaviour
{
    public Transform player;          // �÷��̾� Transform
    public float moveSpeed = 2f;      // �̵� �ӵ�
    public float detectionRange = 10f; // ���� �Ÿ�

    private bool isChasing = false;

    void Start()
    {
        // �÷��̾� �ڵ� Ž�� (������ �±׷� ã��)
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

        // ���� ���� �ȿ� ������ �ѱ� ����
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
                // �÷��̾ ���� ���� �ö���� �� �� ���� + ������ ƨ�ܳ���
                Vector2 knockbackDir;

                // ������ ��ġ ����, �÷��̾ ��� �ʿ� �ִ��� ���
                if (other.transform.position.x >= transform.position.x)
                    knockbackDir = new Vector2(1f, 1f);  // ������ ���� ƨ��
                else
                    knockbackDir = new Vector2(-1f, 1f); // ���� ���� ƨ��

                float knockbackForce = 20f; // ���� ���� (���� ����)
                rb.linearVelocity = Vector2.zero; // ���� �ӵ� �ʱ�ȭ
                rb.AddForce(knockbackDir.normalized * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }


    void MoveTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position);

        // 2D Ⱦ��ũ�� ����: ����(x��)�� �̵�
        direction.y = 0;

        // ����ȭ �� �̵�
        direction = direction.normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;

        // ������ �÷��̾� ������ �ٶ󺸰� (�¿� ������)
        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }
}
