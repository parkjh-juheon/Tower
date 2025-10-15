using UnityEngine;

public class BossChase : MonoBehaviour
{
    [Header("�÷��̾� ����")]
    public Transform player;
    public float moveSpeed = 2f;
    public float stopDistance = 3f; // ���� ��Ÿ� �ȿ��� ����

    [Header("�߼Ҹ� ����")]
    public AudioClip footstepSFX;       // �߼Ҹ� Ŭ��
    public float footstepInterval = 0.6f; // �߼Ҹ� ���� (��)

    private bool isChasing = false;
    private bool reachedAttackRange = false;
    private bool isMoving = false;
    private float nextFootstepTime = 0f;

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

        // ������ �÷��̾� �浹 ����
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

        // �÷��̾ �ָ� ������ �̵�
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
        HandleFootstepSound();
    }

    void MoveTowards(Vector3 target)
    {
        Vector2 dir = (target - transform.position).normalized;

        // 2D Ⱦ��ũ�� �����̶�� �Ʒ� �� �� ����
        dir.y = 0;

        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);

        // �¿� ���� ��ȯ
        if (dir.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (dir.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
    }

    void HandleFootstepSound()
    {
        if (isMoving && Time.time >= nextFootstepTime)
        {
            // �߼Ҹ� ���
            AudioManager.Instance?.PlaySFX(footstepSFX);
            nextFootstepTime = Time.time + footstepInterval;
        }
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
