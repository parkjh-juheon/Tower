using UnityEngine;
using static PlayerController;

public class AttackChangeItem : MonoBehaviour
{
    public AttackType newAttackType = AttackType.Ranged;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController controller = other.GetComponent<PlayerController>();
            if (controller != null)
            {
                // ���� �÷��̾� ��Ʈ�ѷ��� �ִ� attackType ���� ����
                controller.attackType = newAttackType;

                Debug.Log("�÷��̾� ���� ��� ���� �� " + newAttackType);
            }

            // �������� ���� �� ����
            Destroy(gameObject);
        }
    }
}
