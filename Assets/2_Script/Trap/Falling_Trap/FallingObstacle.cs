using UnityEngine;

public class FallingObstacle : MonoBehaviour
{
    public int damage = 15;
    public float knockbackPower = 8f;
    [HideInInspector] public float fallSpeed = 5f; // 스포너에서 설정됨

    private void Update()
    {
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage, transform.position, knockbackPower);

            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
