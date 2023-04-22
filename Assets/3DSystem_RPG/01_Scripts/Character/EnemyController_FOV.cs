using System.Collections;
using UnityEngine;

/// <summary>
/// EnemyController拡張
/// 視野を管理、近い対象を探してターゲットに指定
/// </summary>
public class EnemyController_FOV : EnemyController
{
    #region [Var]
    protected StateMachine<EnemyController> stateMachine;

    public LayerMask targetMask;
    public Collider weaponCollider;
    public GameObject hitEffect;
    private FieldOfView _fov;
    #endregion

    #region [Properties]
    public override Transform Target => _fov?.NearestTarget;
    #endregion

    #region [Unity Methods]
    protected override void Awake()
    {
        base.Awake();

        stateMachine = new StateMachine<EnemyController>(this, new IdleState());
        stateMachine.AddState(new MoveState());
        stateMachine.AddState(new AttackState());

        _fov = GetComponent<FieldOfView>();
    }

    private void Update()
    {
        stateMachine.Update(Time.deltaTime);

        if (!(stateMachine.CurrentState is MoveState))
            FaceTarget();
    }

    #endregion
    
    public R ChangeState<R>() where R : State<EnemyController> 
        => stateMachine.ChangeState<R>();

    public override bool IsAvailableAttack
    {
        get
        {
            if (!Target)
                return false;

            float distance = Vector3.Distance(transform.position, Target.position);
            return (distance <= CalcAttackRange);
        }
    }

    public override Transform SearchEnemy() => Target;

    public void FaceTarget()
    {
        if (!Target)
            return;

        Vector3 direction = (Target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void EnableAttackCollider()
    {
        Debug.Log("Check Attack Event");
        if (weaponCollider)
            weaponCollider.enabled = true;

        StartCoroutine("DisableAttackCollider");
    }

    private IEnumerator DisableAttackCollider()
    {
        yield return new WaitForFixedUpdate();

        if (weaponCollider)
            weaponCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetMask) != 0)
        {
            //It matched one
            Debug.Log("Attack Trigger: " + other.name);
            PlayerCharacter playerCharacter = other.gameObject.GetComponent<PlayerCharacter>();
            playerCharacter?.TakeDamage(10, hitEffect);
        }

        if (((1 << other.gameObject.layer) & targetMask) == 0)
        {
            //It wasn't in an ignore layer
        }
    }

    private void OnDrawGizmos() { }
}