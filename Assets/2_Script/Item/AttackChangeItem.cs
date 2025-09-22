using UnityEngine;
using static PlayerController;

public class AttackChangeItem : MonoBehaviour
{
    public AttackType newAttackType = AttackType.Ranged;
    public int giveAmmo = 6;  // Ranged ��ȯ �� ������ ź�� (RapidFire�� ����)

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController controller = other.GetComponent<PlayerController>();
            if (controller != null)
            {
                // PlayerController���� ���� Ÿ�� ��ȯ ���� ����
                controller.SetAttackType(newAttackType);

                // Ranged Ÿ���� ��츸 ź�� ���� ����
                if (newAttackType == AttackType.Ranged)
                {
                    controller.maxAmmo = giveAmmo;
                    controller.SendMessage("Reload");  // �ʱ� ź�� ä���
                }

                Debug.Log("�÷��̾� ���� ��� ���� �� " + newAttackType);
            }

            Destroy(gameObject);
        }
    }
}
