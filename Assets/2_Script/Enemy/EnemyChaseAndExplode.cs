using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyHealth))]
public class EnemyChaseAndExplode : MonoBehaviour
{
    [Header("탐지 설정")]
    public float detectionRange = 6f;
    public float explosionRange = 1.2f;

    [Header("이동 설정")]
    public float moveSpeed = 2f;
    public float chaseSpeed = 5f;
    public float returnSpeed = 2f;
    public Transform[] patrolPoints;
    private int currentPoint = 0;

    [Header("폭발 설정")]
    public int damage = 30;
    public float knockbackForce = 6f;
    public GameObject explosionPrefab;

    private Rigidbody2D rb;
    private Transform target;
    private EnemyHealth enemyHealth;

    private bool isChasing = false;
    private bool isExploding = false;

    private Vector3 startPos;
    private float chaseCooldown = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyHealth = GetComponent<EnemyHealth>();
        startPos = transform.position;

        // Player 타겟 자동 연결
        target = PlayerController.TargetPoint;

        if (enemyHealth != null)
            enemyHealth.onDie += Explode;
    }

    void FixedUpdate()
    {
        chaseCooldown -= Time.fixedDeltaTime;
        if (isExploding || target == null) return;

        // 플레이어 비활성화 시 타겟 해제
        if (!target.gameObject.activeInHierarchy)
        {
            target = null;
            return;
        }

        float distToPlayer = Vector2.Distance(transform.position, target.position);

        if (distToPlayer <= explosionRange)
        {
            Explode();
            return;
        }

        if (chaseCooldown <= 0f)
        {
            if (distToPlayer <= detectionRange)
            {
                isChasing = true;
            }
            else if (isChasing && distToPlayer > detectionRange * 1.5f)
            {
                isChasing = false;
                chaseCooldown = 1f;
            }
        }

        if (isChasing)
            ChasePlayer();
        else
            Patrol();
    }

    void ChasePlayer()
    {
        if (target == null) return;

        Vector2 dir = ((Vector2)target.position - rb.position).normalized;
        rb.MovePosition(rb.position + dir * chaseSpeed * Time.fixedDeltaTime);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rb.rotation = angle;
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        Transform targetPoint = patrolPoints[currentPoint];
        Vector2 dir = ((Vector2)targetPoint.position - rb.position).normalized;
        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rb.rotation = angle;

        if (Vector2.Distance(rb.position, targetPoint.position) < 0.2f)
        {
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
        }
    }

    public void Explode()
    {
        if (isExploding) return;
        isExploding = true;

        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRange);
        foreach (var hit in hits)
        {
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(damage, transform.position, knockbackForce);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
