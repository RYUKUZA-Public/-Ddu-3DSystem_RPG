using UnityEngine;

/// <summary>
/// 攻撃状態
/// </summary>
public class AttackState : State<EnemyController>
{
    private Animator _animator;
    protected int hashAttack = Animator.StringToHash("Attack");
    
    public override void OnInitialized() => _animator = context.GetComponent<Animator>();

    public override void OnEnter()
    {
        if (context.IsAvailableAttack)
            _animator?.SetTrigger(hashAttack);
        else
            stateMachine.ChangeState<IdleState>();
    }

    public override void Update(float deltaTime) { }
}
