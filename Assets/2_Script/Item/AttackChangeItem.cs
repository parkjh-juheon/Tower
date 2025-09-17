using UnityEngine;
using static PlayerController;

public class AttackChangeItem : MonoBehaviour
{
    public AttackType newAttackType = AttackType.Ranged;
    public int giveAmmo = 6;  //  전환 시 지급할 탄약 수

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController controller = other.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.attackType = newAttackType;

                if (newAttackType == AttackType.Ranged)
                {
                    controller.maxAmmo = giveAmmo;      // 최대 탄약 지정
                    controller.SendMessage("Reload");  // 초기 탄약 채우기
                }

                Debug.Log("플레이어 공격 방식 변경 → " + newAttackType);
            }

            Destroy(gameObject);
        }
    }
}
