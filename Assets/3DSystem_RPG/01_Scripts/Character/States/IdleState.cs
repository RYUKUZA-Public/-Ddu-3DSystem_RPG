using UnityEngine;

/// <summary>
/// 待機状態
/// </summary>
public class IdleState : State<EnemyController>
{
    private bool _isPatrol = false;
    private float _minIdleTime = 0.0f;
    private float _maxIdleTime = 3.0f;
    private float _idleTime = 0.0f;

    private Animator _animator;
    private CharacterController _controller;

    protected int hashMove = Animator.StringToHash("Move");
    protected int hashMoveSpeed = Animator.StringToHash("MoveSpeed");

    public override void OnInitialized()
    {
        _animator = context.GetComponent<Animator>();
        _controller = context.GetComponent<CharacterController>();
    }

    public override void OnEnter()
    {
        _animator?.SetBool(hashMove, false);
        _animator.SetFloat(hashMoveSpeed, 0);
        _controller?.Move(Vector3.zero);

        if (context is EnemyController_Patrol)
        {
            _isPatrol = true;
            _idleTime = Random.Range(_minIdleTime, _maxIdleTime);
        }
    }

    public override void Update(float deltaTime)
    {
        // if searched target
        // change to move state
        Transform enemy = context.SearchEnemy();
        if (enemy)
        {
            // check attack cool time
            // and transition to attack state
            if (context.IsAvailableAttack)
                stateMachine.ChangeState<AttackState>();
            else
                stateMachine.ChangeState<MoveState>();
        }
        else if (_isPatrol && stateMachine.ElapsedTimeInState > _idleTime)
            stateMachine.ChangeState<MoveToWaypointState>();
    }

    public override void OnExit() { }
}