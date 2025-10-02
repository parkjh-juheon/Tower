using UnityEngine;

public class StatItem : MonoBehaviour
{
    [Header("���� ���� ��ȭ��")]
    public float attackDamageBonus = 0;
    public float attackCooldownBonus = 0;
    public float moveSpeedBonus = 0;
    public float jumpForceBonus = 0;
    public int maxHPBonus = 0;
    public int healAmount = 0;                
    public int maxJumpCountBonus = 0;         

    [Header("����(Melee) ���� ���� ��ȭ��")]
    public float meleeAttackRangeBonus = 0;
    public float knockbackBonus = 0;

    [Header("���Ÿ�(Ranged) ���� ���� ��ȭ��")]
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

                // ���� ���� ����
                stats.attackDamage += attackDamageBonus;
                stats.attackCooldown = Mathf.Max(0.1f, stats.attackCooldown + attackCooldownBonus);
                stats.moveSpeed += moveSpeedBonus;
                stats.jumpForce += jumpForceBonus;

                //  ���� Ƚ�� ����
                if (maxJumpCountBonus != 0)
                {
                    stats.maxJumpCount += maxJumpCountBonus;
                }

                // �ִ� ü�� ����
                if (maxHPBonus != 0 && health != null)
                {
                    health.UpdateMaxHP(health.maxHP + maxHPBonus);
                    Debug.Log($"Max HP increased by {maxHPBonus}. New Max HP: {health.maxHP}");
                }

                //  ü�� ȸ��
                if (healAmount > 0 && health != null)
                {
                    health.currentHP = Mathf.Min(health.currentHP + healAmount, health.maxHP);
                    health.UpdateHealthBar();
                }

                // ���� Ÿ�Ժ� ����
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
