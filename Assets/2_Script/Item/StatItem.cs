using UnityEngine;

public class StatItem : MonoBehaviour
{
    [Header("공통 스탯 변화량")]
    public float attackDamageBonus = 0;
    public float attackCooldownBonus = 0;
    public float moveSpeedBonus = 0;
    public float jumpForceBonus = 0;
    public int maxHPBonus = 0;
    public int healAmount = 0;                
    public int maxJumpCountBonus = 0;         

    [Header("근접(Melee) 전용 스탯 변화량")]
    public float meleeAttackRangeBonus = 0;
    public float knockbackBonus = 0;

    [Header("원거리(Ranged) 전용 스탯 변화량")]
    public float bulletSpeedBonus = 0;
    public float bulletSizeBonus = 0;
    public float bulletLifeTimeBonus = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController controller = other.GetComponent<PlayerController>();
            PlayerHealth health = other.GetComponent<PlayerHealth>();

            if (controller != null && controller.stats != null)
            {
                var stats = controller.stats;

                // 공통 스탯 적용
                stats.attackDamage += attackDamageBonus;
                stats.attackCooldown = Mathf.Max(0.1f, stats.attackCooldown + attackCooldownBonus);
                stats.moveSpeed += moveSpeedBonus;
                stats.jumpForce += jumpForceBonus;

                //  점프 횟수 증가
                if (maxJumpCountBonus != 0)
                {
                    stats.maxJumpCount += maxJumpCountBonus;
                }

                // 최대 체력 증가
                if (maxHPBonus != 0 && health != null)
                {
                    health.UpdateMaxHP(health.maxHP + maxHPBonus);
                    Debug.Log($"Max HP increased by {maxHPBonus}. New Max HP: {health.maxHP}");
                }

                //  체력 회복
                if (healAmount > 0 && health != null)
                {
                    health.currentHP = Mathf.Min(health.currentHP + healAmount, health.maxHP);
                    health.UpdateHealthBar();
                }

                // 공격 타입별 적용
                if (controller.attackType == PlayerController.AttackType.Melee)
                {
                    stats.meleeRange += meleeAttackRangeBonus;
                    stats.knockbackPower += knockbackBonus;
                }
                else if (controller.attackType == PlayerController.AttackType.Ranged)
                {
                    stats.bulletSpeed += bulletSpeedBonus;
                    stats.bulletSize += bulletSizeBonus;
                    stats.bulletLifeTime += bulletLifeTimeBonus;
                }
            }

            Destroy(gameObject);
        }
    }
}
