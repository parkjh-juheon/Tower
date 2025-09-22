using UnityEngine;
using static PlayerController;

public class AttackChangeItem : MonoBehaviour
{
    public AttackType newAttackType = AttackType.Ranged;
    public int giveAmmo = 6;  // Ranged 전환 시 지급할 탄약 (RapidFire는 무시)

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController controller = other.GetComponent<PlayerController>();
            if (controller != null)
            {
                // PlayerController에서 공격 타입 전환 로직 실행
                controller.SetAttackType(newAttackType);

                // Ranged 타입일 경우만 탄약 개별 조정
                if (newAttackType == AttackType.Ranged)
                {
                    controller.maxAmmo = giveAmmo;
                    controller.SendMessage("Reload");  // 초기 탄약 채우기
                }

                Debug.Log("플레이어 공격 방식 변경 → " + newAttackType);
            }

            Destroy(gameObject);
        }
    }
}
