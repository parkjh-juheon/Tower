using UnityEngine;
using static PlayerController;

public class AttackChangeItem : MonoBehaviour
{
    public ItemData data;  
    public AttackType newAttackType = AttackType.Ranged;
    public int giveAmmo = 6;  // Ranged 전환 시 지급할 탄약 (RapidFire는 무시)

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController controller = other.GetComponent<PlayerController>();

            if (controller != null)
            {
                controller.SetAttackType(newAttackType);

                if (newAttackType == AttackType.Ranged)
                {
                    controller.maxAmmo = giveAmmo;
                    controller.SendMessage("Reload");
                }

                Debug.Log("플레이어 공격 방식 변경 → " + newAttackType);

                if (ItemInventory.Instance != null && data != null)
                    ItemInventory.Instance.AddItem(data);
            }

            Destroy(gameObject);
        }
    }
}
