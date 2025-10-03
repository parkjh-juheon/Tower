using UnityEngine;

public class StatItem : MonoBehaviour
{
    public ItemData data;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController controller = other.GetComponent<PlayerController>();
            PlayerHealth health = other.GetComponent<PlayerHealth>();

            if (controller != null && controller.stats != null)
            {
                var stats = controller.stats;

                // ���� ���� ����
                stats.moveSpeed += data.moveSpeedBonus;
                stats.dashDistance += data.dashDistanceBonus;
                stats.jumpForce += data.jumpForceBonus;
                stats.maxJumpCount += data.maxJumpCountBonus;
                stats.attackDamage += data.attackDamageBonus;
                stats.attackCooldown = Mathf.Max(0.05f, stats.attackCooldown + data.attackCooldownBonus);

                if (data.maxHPBonus > 0 && health != null)
                    health.UpdateMaxHP(health.maxHP + data.maxHPBonus);

                if (data.healAmount > 0 && health != null)
                {
                    health.currentHP = Mathf.Min(health.currentHP + data.healAmount, health.maxHP);
                    health.UpdateHealthBar();
                }

                // ���� ����
                if (controller.attackType == PlayerController.AttackType.Melee)
                {
                    stats.knockbackPower += data.knockbackPowerBonus;
                    stats.meleeRange += data.meleeRangeBonus;
                }

                // ���Ÿ� ����
                if (controller.attackType == PlayerController.AttackType.Ranged ||
                    controller.attackType == PlayerController.AttackType.RapidFire)
                {
                    stats.bulletSize += data.bulletSizeBonus;
                    stats.bulletLifeTime += data.bulletLifeTimeBonus;
                    stats.bulletSpeed += data.bulletSpeedBonus;
                }
            }

            Debug.Log($"ȹ���� ������: {data.itemName} (��͵�: {data.rarity}) - {data.description}");
            Destroy(gameObject);
        }
    }
}
