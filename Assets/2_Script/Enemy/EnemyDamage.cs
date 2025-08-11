using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damage = 10;              // Player���� �� ������
    public float knockbackPower = 5f;    // �÷��̾� �˹� ����

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // �÷��̾�� �������� �˹� ���� ����
                playerHealth.TakeDamage(damage, transform.position, knockbackPower);
            }
        }
    }
}
