using UnityEngine;

public class StatItem : MonoBehaviour
{
    [Header("½ºÅÈ º¯È­·®")]
    public float attackDamageBonus = 0;
    public float attackCooldownBonus = 0;
    public float attackRangeBonus = 0;
    public float moveSpeedBonus = 0;
    public float jumpForceBonus = 0;
    public int maxHPBonus = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController controller = other.GetComponent<PlayerController>();
            PlayerHealth health = other.GetComponent<PlayerHealth>();

            if (controller != null)
            {
                controller.attackDamage += (int)attackDamageBonus;
                controller.attackCooldown = Mathf.Max(0.1f, controller.attackCooldown + attackCooldownBonus); // ¾ç¼ö=´À·ÁÁü, À½¼ö=»¡¶óÁü
                controller.attackRange += attackRangeBonus;
                controller.moveSpeed += moveSpeedBonus;
                controller.jumpForce += jumpForceBonus;
            }

            if (health != null)
            {
                health.UpdateMaxHP(health.maxHP + maxHPBonus);
            }

            Destroy(gameObject);
        }
    }

}
