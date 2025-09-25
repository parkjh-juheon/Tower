using UnityEngine;

public class AirAttackLogger : StateMachineBehaviour
{
    // AirAttack_Falling 상태로 들어갈 때 호출됨
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"[{Time.time:F2}] [Animator] AirAttack_Falling 애니메이션 시작");
    }

    // AirAttack_Falling 상태에서 나갈 때 호출됨
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"[{Time.time:F2}] [Animator] AirAttack_Falling 애니메이션 종료");
    }
}
