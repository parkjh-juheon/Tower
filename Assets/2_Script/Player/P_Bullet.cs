using UnityEngine;

public class P_Bullet : MonoBehaviour
{
    private int damage;
    private float speed;
    private float size;
    private float lifeTime;

    private Rigidbody2D rb;

    [Header("이펙트 설정")]
    public GameObject hitEffectPrefab; // 총알이 맞았을 때 나오는 이펙트

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage, transform.position, 3f);
            }
            SpawnHitEffect();
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            SpawnHitEffect();
            Destroy(gameObject);
        }
    }

    public void Init(int damage, float speed, float size, float lifeTime, int facingDirection)
    {
        this.damage = damage;
        this.speed = speed;
        this.size = size;
        this.lifeTime = lifeTime;

        transform.localScale = Vector3.one * size;
        rb.linearVelocity = new Vector2(speed * facingDirection, 0f);
        Destroy(gameObject, lifeTime);
    }

    private void SpawnHitEffect()
    {
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 1f); // 1초 후 이펙트 삭제
        }
    }
}
