using UnityEngine;

/// <summary>
/// 敵Monsterが、攻撃状態から抜け出す時に呼び出す。
/// </summary>
public class AttackStateMachineBehaviour : StateMachineBehaviour
{
     public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<EnemyController_StateMachine>()?.ChangeState<IdleState>();
        animator.gameObject.GetComponent<EnemyController_FOV>()?.ChangeState<IdleState>();
        animator.gameObject.GetComponent<EnemyController_Patrol>()?.ChangeState<IdleState>();
    }
}
