using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class Missile : MonoBehaviour
{
    public float speed = 6f;
    public GameObject explosionPrefab;

    private Transform target;
    private EnemyHealth enemyHealth;

    [Header("������Ÿ�� ����")]
    public float lifeTime = 5f;
    private float spawnTime;

    void Start()
    {
        spawnTime = Time.time;
        enemyHealth = GetComponent<EnemyHealth>();

        // EnemyHealth�� Die()�� ȣ��Ǹ� ���ߵǵ��� ����
        if (enemyHealth != null)
        {
            // EnemyHealth���� ���� Destroy���� ���� Missile�� ���� ó���ϵ��� ����
        }
    }

    public void SetTarget(Transform t) => target = t;

    void Update()
    {
        // ������Ÿ�� �ʰ� �� ����
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

        // ���� �̵�
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
