using System;
using UnityEngine;

public class P_Bullet : MonoBehaviour
{
    public float lifeTime = 1f;

    private int damage;
    private float speed;
    private float size;

    void Start()
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
                enemyHealth.TakeDamage(damage, transform.position, 3f); // ������ 1, �˹� �Ŀ� 3f
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

    public void Init(int damage, float speed, float size)
    {
        this.damage = damage;
        this.speed = speed;
        this.size = size;

        // �Ѿ� ũ�� ����
        transform.localScale *= size;

        // �Ѿ� �ӵ� ����
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * speed;
    }

    internal void Init(float attackDamage, float bulletSpeed, float bulletSize)
    {
        throw new NotImplementedException();
    }
}