using UnityEngine;

public class AirAttackLogger : StateMachineBehaviour
{
    // AirAttack_Falling ���·� �� �� ȣ���
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"[{Time.time:F2}] [Animator] AirAttack_Falling �ִϸ��̼� ����");
    }

    // AirAttack_Falling ���¿��� ���� �� ȣ���
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"[{Time.time:F2}] [Animator] AirAttack_Falling �ִϸ��̼� ����");
    }
}
