using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public Transform player;              // ������ ���
    public float moveSpeed = 2f;          // �̵� �ӵ�
    public float detectionRange = 5f;     // �ν� �Ÿ�

    private bool isChasing = false;

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // �ν� �Ÿ� �ȿ� �÷��̾ �ִ��� üũ
        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }

        // ����
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
