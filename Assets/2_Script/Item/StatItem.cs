using UnityEngine;

public class StatItem : MonoBehaviour
{
    public StatItemData data; // 아이템 데이터 참조

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && data != null)
        {
            PlayerController controller = other.GetComponent<PlayerController>();
            PlayerHealth health = other.GetComponent<PlayerHealth>();

            if (controller != null && controller.stats != null)
            {
                var stats = controller.stats;

                // 공통 스탯 적용
                stats.attackDamage += data.attackDamageBonus;
                stats.attackCooldown = Mathf.Max(0.1f, stats.attackCooldown + data.attackCooldownBonus);
                stats.moveSpeed += data.moveSpeedBonus;
                stats.jumpForce += data.jumpForceBonus;

                if (data.maxJumpCountBonus != 0)
                    stats.maxJumpCount += data.maxJumpCountBonus;

                if (data.maxHPBonus != 0)
                {
                    stats.maxHP += data.maxHPBonus;
                    if (health != null)
                        health.UpdateMaxHP(health.maxHP + data.maxHPBonus);
                }

                if (data.healAmount > 0 && health != null)
                {
                    health.currentHP = Mathf.Min(health.currentHP + data.healAmount, health.maxHP);
                    health.UpdateHealthBar();
                }

                if (controller.attackType == PlayerController.AttackType.Melee)
                {
                    stats.meleeRange += data.meleeAttackRangeBonus;
                    stats.knockbackPower += data.knockbackBonus;
                }
                else if (controller.attackType == PlayerController.AttackType.Ranged)
                {
                    stats.bulletSpeed += data.bulletSpeedBonus;
                    stats.bulletSize += data.bulletSizeBonus;
                    stats.bulletLifeTime += data.bulletLifeTimeBonus;
                }
            }

            Debug.Log($"플레이어가 [{data.itemName}] 아이템을 획득했습니다!");
            Destroy(gameObject);
        }
    }
}
