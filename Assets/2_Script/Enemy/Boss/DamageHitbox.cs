using UnityEngine;

public class DamageHitbox : MonoBehaviour
{
    public int damage = 20;
    public float knockbackPower = 8f;
    public LayerMask targetLayer; // Player만 맞게 설정

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & targetLayer) == 0) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage, transform.position, knockbackPower);
            }
        }
    }
}
