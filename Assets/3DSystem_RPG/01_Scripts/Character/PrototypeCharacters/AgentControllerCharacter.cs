using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Click and Move方式の移動
/// CharacterControllerとNavMeshAgentを利用
/// </summary>
public class AgentControllerCharacter : MonoBehaviour
{
    #region [Var]
    [SerializeField]
    private LayerMask groundLayerMask;
    
    private CharacterController _characterController;
    private NavMeshAgent _agent;
    private Camera _camera;
    #endregion

    #region [Unity Methods]
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        
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
            _characterController.Move(_agent.velocity * Time.deltaTime);
        else
            _characterController.Move(Vector3.zero);
    }

    private void LateUpdate() => transform.position = _agent.nextPosition;
    #endregion
}
