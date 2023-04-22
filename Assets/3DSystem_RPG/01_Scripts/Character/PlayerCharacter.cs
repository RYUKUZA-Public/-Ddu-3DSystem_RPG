using System;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(CharacterController)), RequireComponent(typeof(Animator))]
public class PlayerCharacter : MonoBehaviour
{
    #region [Var]
    [SerializeField]
    private LayerMask groundLayerMask;
    [SerializeField]
    private Animator animator;
    
    private CharacterController _controller;
    private NavMeshAgent _agent;
    private Camera _camera;

    readonly int _moveHash = Animator.StringToHash("Move");
    readonly int _fallingHash = Animator.StringToHash("Falling");
    #endregion
    
    #region [Unity Methods]
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();

        _agent = GetComponent<NavMeshAgent>();
        _agent.updatePosition = false;
        _agent.updateRotation = true;
        
        _camera = Camera.main;
    }

    private void Update()
    {
        // Process mouse left button input
        if (Input.GetMouseButtonDown(0))
        {
            // Make ray from screen to world
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            // Check hit from ray
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, groundLayerMask))
            {
                Debug.Log("We hit " + hit.collider.name + " " + hit.point);

                // Move our player to what we hit
                _agent.SetDestination(hit.point);
            }
        }

        if (_agent.remainingDistance > _agent.stoppingDistance)
        {
            _controller.Move(_agent.velocity * Time.deltaTime);
            animator.SetBool(_moveHash, true);
        }
        else
        {
            _controller.Move(Vector3.zero);
            animator.SetBool(_moveHash, false);
        }

        // if (_agent.isOnOffMeshLink)
        //     animator.SetBool(_fallingHash, _agent.velocity.y != 0.0f);
        // else
        //     animator.SetBool(_fallingHash, false);
    }
    
    private void OnAnimatorMove()
    {
        Vector3 position = _agent.nextPosition;
        animator.rootPosition = position;
        transform.position = position;
    }
    #endregion
}
