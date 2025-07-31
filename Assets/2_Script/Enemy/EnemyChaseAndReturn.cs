using UnityEngine;

public class EnemyChaseAndReturn : MonoBehaviour
{
    public Transform player;               // �÷��̾� Transform
    public float moveSpeed = 2f;           // �̵� �ӵ�
    public float detectionRange = 5f;      // ���� �Ÿ�
    public float returnStopDistance = 0.1f; // ����ġ�� ������ ������ �Ǵ��� �Ÿ�

    private Vector3 startPosition;         // ó�� ��ġ ����
    private bool isChasing = false;

    void Start()
    {
        // ó�� ��ġ ����
        startPosition = transform.position;

        // �÷��̾� �ڵ� Ž�� (���� ���)
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

        // ����(X��) �������θ� �̵��ϰ� ��
        direction.y = 0;

        // ����ȭ�� �� �ӵ� ���� �̵�
        direction = direction.normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;
    }

}
