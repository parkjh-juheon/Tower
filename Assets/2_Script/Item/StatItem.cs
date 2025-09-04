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
    public float meleeAttackRangeBonus = 0;

    [Header("���Ÿ�(Ranged) ���� ���� ��ȭ��")]
    public float bulletSpeedBonus = 0;  // ź�� ����
    public float bulletSizeBonus = 0;   // �Ѿ� ũ�� ����

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController controller = other.GetComponent<PlayerController>();
            PlayerHealth health = other.GetComponent<PlayerHealth>();

            if (controller != null)
            {
                // ���� ���� ����
                controller.attackDamage += (int)attackDamageBonus;
                controller.attackCooldown = Mathf.Max(0.1f, controller.attackCooldown + attackCooldownBonus);
                controller.moveSpeed += moveSpeedBonus;
                controller.jumpForce += jumpForceBonus;

                if (health != null && maxHPBonus != 0)
                {
                    health.UpdateMaxHP(health.maxHP + maxHPBonus);
                }

                // ���� Ÿ�Ժ� ����
                if (controller.attackType == PlayerController.AttackType.Melee)
                {
                    controller.attackRange += meleeAttackRangeBonus;
                }
                else if (controller.attackType == PlayerController.AttackType.Ranged)
                {
                    controller.bulletSpeed += bulletSpeedBonus;
                    controller.bulletSize += bulletSizeBonus;
                }
            }

            Destroy(gameObject);
        }
    }
}
