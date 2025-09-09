using UnityEngine;

public class StatItem : MonoBehaviour
{
    [Header("공통 스탯 변화량")]
    public float attackDamageBonus = 0;       // 공격력 (공통)
    public float attackCooldownBonus = 0;     // 공격 쿨타임 (공통)
    public float moveSpeedBonus = 0;          // 이동속도
    public float jumpForceBonus = 0;          // 점프력
    public int maxHPBonus = 0;                // 체력

    [Header("근접(Melee) 전용 스탯 변화량")]
    public float meleeAttackRangeBonus = 0;

    [Header("원거리(Ranged) 전용 스탯 변화량")]
    public float bulletSpeedBonus = 0;  // 탄속 증가
    public float bulletSizeBonus = 0;   // 총알 크기 증가

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController controller = other.GetComponent<PlayerController>();
            PlayerHealth health = other.GetComponent<PlayerHealth>();

            if (controller != null)
            {
                // 공통 스탯 적용
                controller.attackDamage += (int)attackDamageBonus;
                controller.attackCooldown = Mathf.Max(0.1f, controller.attackCooldown + attackCooldownBonus);
                controller.moveSpeed += moveSpeedBonus;
                controller.jumpForce += jumpForceBonus;

                if (health != null && maxHPBonus != 0)
                {
                    health.UpdateMaxHP(health.maxHP + maxHPBonus);
                }

                // 공격 타입별 적용
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
