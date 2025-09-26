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
