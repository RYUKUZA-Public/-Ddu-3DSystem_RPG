using UnityEngine;

/// <summary>
/// ランダムでIdleアニメーションを再生する、StateMachineBehaviour
/// </summary>
public class RandomStateMachineBehaviour : StateMachineBehaviour
{
    #region [Var]
    public int numberOfStates = 2;
    public float minNormTime = 0f;
    public float maxNormTime = 5f;
    
    public float randomNormalTime;
    
    private readonly int _hashRandomIdle = Animator.StringToHash("RandomIdle");
    #endregion
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Randomly decide a time at which to transition.
        randomNormalTime = Random.Range(minNormTime, maxNormTime);
    }
    
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If transitioning awy from this state reset the random idle parameter to -1.
        if (animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).fullPathHash == stateInfo.fullPathHash)
            animator.SetInteger(_hashRandomIdle, -1);

        // If the state is beyond the randomly decided normalised time and not yet transitioning then set a random idle.
        if (stateInfo.normalizedTime > randomNormalTime && !animator.IsInTransition(0))
            animator.SetInteger(_hashRandomIdle, Random.Range(0, numberOfStates));
    }
}
