using UnityEngine;

public class EnemyChaseAndReturn : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 2f;
    public float detectionRange = 5f;
    public float returnStopDistance = 0.1f;

    private Vector3 startPosition;
    private bool isChasing = false;
    private SpriteRenderer spriteRenderer; // �� ��������Ʈ ���� �����

    void Start()
    {
        startPosition = transform.position;

        // ��������Ʈ ������ �ڵ� Ž��
        spriteRenderer = GetComponent<SpriteRenderer>();

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
            isChasing = true;
        else if (distanceToPlayer > detectionRange)
            isChasing = false;

        if (isChasing)
            MoveTowards(player.position);
        else if (Vector2.Distance(transform.position, startPosition) > returnStopDistance)
            MoveTowards(startPosition);
    }

    void MoveTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position);
        direction.y = 0;
        direction = direction.normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;

        // �ٶ󺸴� ���� ��ȯ
        if (spriteRenderer != null)
        {
            if (direction.x > 0)
                spriteRenderer.flipX = true; // ������ �ٶ�
            else if (direction.x < 0)
                spriteRenderer.flipX = false;  // ���� �ٶ�
        }
    }
}
