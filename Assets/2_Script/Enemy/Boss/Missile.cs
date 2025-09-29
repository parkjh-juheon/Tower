using UnityEngine;

public class Missile : MonoBehaviour
{
    public float speed = 6f;
    public GameObject explosionPrefab;

    private Vector2 moveDirection = Vector2.zero; // �̵� ����

    // ���� ���� �̻��Ͽ�
    private Transform target;

    public void SetTarget(Transform t) => target = t;

    // ���� �߻��
    public void SetTargetDirection(Vector2 dir)
    {
        moveDirection = dir.normalized;
    }

    void Update()
    {
        // ���� �߻� �켱
        if (moveDirection != Vector2.zero)
        {
            transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            return;
        }

        // ���� ���� �̻���
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

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
        }

        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
