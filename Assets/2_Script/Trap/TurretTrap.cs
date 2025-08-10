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
                // 좌우 방향만 반전
                if (player.position.x < transform.position.x)
                {
                    // 플레이어가 왼쪽
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else
                {
                    // 플레이어가 오른쪽
                    transform.localScale = new Vector3(1, 1, 1);
                }

                // 발사 쿨다운 체크
                if (fireTimer >= fireCooldown)
                {
                    Vector2 direction = (player.position - transform.position).normalized;
                    direction.y = 0; // Y축 고정 (수평 방향)
                    direction.Normalize();

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
