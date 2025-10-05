using UnityEngine;
using static PlayerController;

public class AttackChangeItem : MonoBehaviour
{
    public ItemData data;  
    public AttackType newAttackType = AttackType.Ranged;
    public int giveAmmo = 6;  // Ranged ��ȯ �� ������ ź�� (RapidFire�� ����)

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

                Debug.Log("�÷��̾� ���� ��� ���� �� " + newAttackType);

                if (ItemInventory.Instance != null && data != null)
                    ItemInventory.Instance.AddItem(data);
            }

            Destroy(gameObject);
        }
    }
}
