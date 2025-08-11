using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damage = 10;              // Player에게 줄 데미지
    public float knockbackPower = 5f;    // 플레이어 넉백 세기

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // 플레이어에게 데미지와 넉백 같이 전달
                playerHealth.TakeDamage(damage, transform.position, knockbackPower);
            }
        }
    }
}
