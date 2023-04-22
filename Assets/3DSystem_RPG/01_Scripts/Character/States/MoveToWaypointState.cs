using UnityEngine.AI;
using UnityEngine;

/// <summary>
/// パトロール状態のとき、次の首阻止地点に移動
/// </summary>
public class MoveToWaypointState : State<EnemyController>
    {
        private Animator _animator;
        private CharacterController _controller;
        private NavMeshAgent _agent;

        private EnemyController_Patrol _patrolController;

        private int hashMove = Animator.StringToHash("Move");
        private int hashMoveSpeed = Animator.StringToHash("MoveSpeed");

        public override void OnInitialized()
        {
            _animator = context.GetComponent<Animator>();
            _controller = context.GetComponent<CharacterController>();
            _agent = context.GetComponent<NavMeshAgent>();
            _patrolController = context as EnemyController_Patrol;
        }

        public override void OnEnter()
        {
            _agent.stoppingDistance = 0.0f;

            if (_patrolController?.targetWaypoint == null)
                _patrolController?.FindNextWaypoint();

            if (_patrolController?.targetWaypoint != null)
            {
                Vector3 destination = _patrolController.targetWaypoint.position;
                _agent?.SetDestination(destination);
                _animator?.SetBool(hashMove, true);
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
            else
            {
                if (!_agent.pathPending && (_agent.remainingDistance <= _agent.stoppingDistance))
                {
                    FindNextWaypoint();
                    stateMachine.ChangeState<IdleState>();
                }
                else
                {
                    _controller.Move(_agent.velocity * Time.deltaTime);
                    _animator.SetFloat(hashMoveSpeed, _agent.velocity.magnitude / _agent.speed, .1f, Time.deltaTime);
                }
            }
        }

        public override void OnExit()
        {
            _agent.stoppingDistance = context.attackRange;
            _animator?.SetBool(hashMove, false);
            _agent.ResetPath();
        }

        private void FindNextWaypoint()
        {
            Transform targetWaypoint = _patrolController.FindNextWaypoint();
            if (targetWaypoint != null)
            {
                _agent?.SetDestination(targetWaypoint.position);
            }
        }
    }