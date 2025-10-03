using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class Missile : MonoBehaviour
{
    public float speed = 6f;
    public GameObject explosionPrefab;

    private Transform target;
    private EnemyHealth enemyHealth;

    [Header("라이프타임 설정")]
    public float lifeTime = 5f;
    private float spawnTime;

    void Start()
    {
        spawnTime = Time.time;
        enemyHealth = GetComponent<EnemyHealth>();

        // EnemyHealth의 Die()가 호출되면 폭발되도록 연결
        if (enemyHealth != null)
        {
            // EnemyHealth에서 직접 Destroy하지 말고 Missile이 폭발 처리하도록 유도
        }
    }

    public void SetTarget(Transform t) => target = t;

    void Update()
    {
        // 라이프타임 초과 시 폭발
        if (Time.time - spawnTime >= lifeTime)
        {
            Explode();
            return;
        }

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // 유도 이동
        Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(dir * speed * Time.deltaTime);

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(20, transform.position, 5f);
            }
            Explode();
        }
    }

    public void Explode()
    {
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
