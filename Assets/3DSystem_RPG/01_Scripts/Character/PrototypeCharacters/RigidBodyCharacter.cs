using UnityEngine;

/// <summary>
/// キャラクターの移動、ジャンプ、ダッシュ
/// 地面にいる場合にのみ、Jumpを可能にする。
/// Dashは、加速後加減する。
/// </summary>
public class RigidBodyCharacter : MonoBehaviour
{
    #region [Var]
    [SerializeField] public float speed = 5f;
    [SerializeField] public float jumpHeight = 2f;
    [SerializeField] public float dashDistance = 5f;
    private Rigidbody _rigidbody;
    private Vector3 _inputDirection = Vector3.zero;
    
    [SerializeField]
    private LayerMask groundLayerMask;
    [SerializeField]
    private float groundCheckDistance = 0.2f;
    private bool _isGrounded = true;
    #endregion

    private void Awake() => _rigidbody = GetComponent<Rigidbody>();

    void Update()
    {
        // Check grounded
        CheckGroundStatus();
        
        // Process move inputs
        _inputDirection = Vector3.zero;
        _inputDirection.x = Input.GetAxis("Horizontal");
        _inputDirection.z = Input.GetAxis("Vertical");
        
        if (_inputDirection != Vector3.zero)
            transform.forward = _inputDirection;

        // Process jump input
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            Vector3 jumpVelocity = Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
            _rigidbody.AddForce(jumpVelocity, ForceMode.VelocityChange);
        }
         
        // Process Dash input
        if (Input.GetButtonDown("Dash"))
        {
            Vector3 dashVelocity = Vector3.Scale(
                transform.forward,
                dashDistance * new Vector3(
                    (Mathf.Log(1f / (Time.deltaTime * _rigidbody.drag + 1)) / -Time.deltaTime),
                    0,
                    (Mathf.Log(1f / (Time.deltaTime * _rigidbody.drag + 1)) / -Time.deltaTime)));
            
            _rigidbody.AddForce(dashVelocity, ForceMode.VelocityChange);
        }
    }

    private void FixedUpdate() => 
        _rigidbody.MovePosition(_rigidbody.position + _inputDirection * speed * Time.fixedDeltaTime);

    #region [Unity Methods]
    void CheckGroundStatus()
    {
        RaycastHit hitInfo;
        
#if UNITY_EDITOR
        // Ground check ray in the scene view
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), 
            transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance));
#endif
        
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), 
                Vector3.down, out hitInfo, groundCheckDistance, groundLayerMask))
            _isGrounded = true;
        else
            _isGrounded = false;
    }
    #endregion 
}
