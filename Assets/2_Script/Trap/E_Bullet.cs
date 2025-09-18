using UnityEngine;

public class E_Bullet : MonoBehaviour
{
    public float lifeTime = 1f;
    public int damage = 10;
    public float knockbackPower = 8f; // ���� 3f �� 8f ������ ����

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
                // �Ѿ��� ��ġ�� �������� �˹� ���� ���
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
