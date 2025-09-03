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
                // 현재 플레이어 컨트롤러에 있는 attackType 변수 수정
                controller.attackType = newAttackType;

                Debug.Log("플레이어 공격 방식 변경 → " + newAttackType);
            }

            // 아이템은 먹은 후 삭제
            Destroy(gameObject);
        }
    }
}
