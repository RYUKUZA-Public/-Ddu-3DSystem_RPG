using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 移動状態
/// </summary>
public class MoveState : State<EnemyController>
{
    private Animator _animator;
    private CharacterController _controller;
    private NavMeshAgent _agent;

    private int hashMove = Animator.StringToHash("Move");
    private int hashMoveSpeed = Animator.StringToHash("MoveSpeed");

    public override void OnInitialized()
    {
        _animator = context.GetComponent<Animator>();
        _controller = context.GetComponent<CharacterController>();
        _agent = context.GetComponent<NavMeshAgent>();
    }

    public override void OnEnter()
    {
        _agent?.SetDestination(context.Target.position);
        _animator?.SetBool(hashMove, true);
    }

    public override void Update(float deltaTime)
    {
        Transform enemy = context.SearchEnemy();
        if (enemy)
        {
            _agent.SetDestination(context.Target.position);
            if (_agent.remainingDistance > _agent.stoppingDistance)
            {
                _controller.Move(_agent.velocity * Time.deltaTime);
                _animator.SetFloat(hashMoveSpeed, _agent.velocity.magnitude / _agent.speed, .1f, Time.deltaTime);
                return;
            }
        }

        stateMachine.ChangeState<IdleState>();
    }

    public override void OnExit()
    {
        _animator?.SetBool(hashMove, false);
        _agent.ResetPath();
    }
}