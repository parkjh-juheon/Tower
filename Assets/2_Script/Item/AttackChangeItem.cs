using UnityEngine;
using static PlayerController;

public class AttackChangeItem : MonoBehaviour
{
    public AttackType newAttackType = AttackType.Ranged;
    public int giveAmmo = 6;  //  ��ȯ �� ������ ź�� ��

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
                    controller.maxAmmo = giveAmmo;      // �ִ� ź�� ����
                    controller.SendMessage("Reload");  // �ʱ� ź�� ä���
                }

                Debug.Log("�÷��̾� ���� ��� ���� �� " + newAttackType);
            }

            Destroy(gameObject);
        }
    }
}
