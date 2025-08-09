using UnityEngine;

public class TurretTrap : MonoBehaviour
{
    [Header("탐지 설정")]
    public float triggerRadius = 5f;

    [Header("발사 설정")]
    public GameObject bulletPrefab;
    public Transform firePoint; //총구 위치
    public float bulletSpeed = 10f;
    public float fireCooldown = 1.5f;

    private Transform player;
    private float fireTimer = 0f;

    void Update()
    {
        if (player != null)
        {
            fireTimer += Time.deltaTime;

            // 플레이어까지 거리 계산
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= triggerRadius)
            {
                // 포탑 회전 (선택)
                Vector2 direction = (player.position - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);

                // 발사 쿨다운 체크
                if (fireTimer >= fireCooldown)
                {
                    Shoot(direction);
                    fireTimer = 0f;
                }
            }
        }
    }

    void Shoot(Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * bulletSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}
