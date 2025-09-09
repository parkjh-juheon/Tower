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
    public float meleeAttackRangeBonus = 0;   // 근접 공격 범위
    public float knockbackBonus = 0;          // 넉백

    [Header("원거리(Ranged) 전용 스탯 변화량")]
    public float bulletSpeedBonus = 0;        // 탄속 증가
    public float bulletSizeBonus = 0;         // 총알 크기 증가
    public float bulletLifeTimeBonus = 0;     // 지속 시간 증가

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

                if (maxHPBonus != 0)
                {
                    stats.maxHP += maxHPBonus;

                    // PlayerHealth에도 반영
                    if (health != null)
                    {
                        health.UpdateMaxHP(health.maxHP + maxHPBonus);
                    }
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

            Destroy(gameObject); // 아이템 획득 후 제거
        }
    }
}
