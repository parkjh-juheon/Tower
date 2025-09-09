using UnityEngine;

public class StatItem : MonoBehaviour
{
    [Header("���� ���� ��ȭ��")]
    public float attackDamageBonus = 0;       // ���ݷ� (����)
    public float attackCooldownBonus = 0;     // ���� ��Ÿ�� (����)
    public float moveSpeedBonus = 0;          // �̵��ӵ�
    public float jumpForceBonus = 0;          // ������
    public int maxHPBonus = 0;                // ü��

    [Header("����(Melee) ���� ���� ��ȭ��")]
    public float meleeAttackRangeBonus = 0;   // ���� ���� ����
    public float knockbackBonus = 0;          // �˹�

    [Header("���Ÿ�(Ranged) ���� ���� ��ȭ��")]
    public float bulletSpeedBonus = 0;        // ź�� ����
    public float bulletSizeBonus = 0;         // �Ѿ� ũ�� ����
    public float bulletLifeTimeBonus = 0;     // ���� �ð� ����

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

                if (maxHPBonus != 0)
                {
                    stats.maxHP += maxHPBonus;

                    // PlayerHealth���� �ݿ�
                    if (health != null)
                    {
                        health.UpdateMaxHP(health.maxHP + maxHPBonus);
                    }
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

            Destroy(gameObject); // ������ ȹ�� �� ����
        }
    }
}
