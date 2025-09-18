using UnityEngine;

public class E_Bullet : MonoBehaviour
{
    public float lifeTime = 1f;
    public int damage = 10;
    public float knockbackPower = 8f; // 기존 3f → 8f 정도로 상향

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // 총알의 위치를 기준으로 넉백 방향 계산
                playerHealth.TakeDamage(damage, transform.position, knockbackPower);
            }

            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
