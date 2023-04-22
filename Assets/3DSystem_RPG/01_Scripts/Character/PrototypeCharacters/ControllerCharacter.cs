using UnityEngine;

/// <summary>
/// キャラクターの移動、ジャンプ、ダッシュ
/// CharacterControllerを使用
/// </summary>
public class ControllerCharacter : MonoBehaviour
{
    #region [Var]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private Vector3 drags;
    
    private CharacterController _characterController;
    private Vector3 _calcVelocity = Vector3.zero;
    #endregion

    #region [Unity Methods]
    private void Awake() => _characterController = GetComponent<CharacterController>();

    private void Update()
    {
        // Check grounded
        bool isGrounded = _characterController.isGrounded;
        if (isGrounded && _calcVelocity.y < 0)
            _calcVelocity.y = 0f;

        // Process move inputs
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        _characterController.Move(move * Time.deltaTime * speed);
        
        if (move != Vector3.zero)
            transform.forward = move;

        // Process jump input
        if (Input.GetButtonDown("Jump") && isGrounded)
            _calcVelocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Process Dash input
        if (Input.GetButtonDown("Dash"))
        {
            _calcVelocity += Vector3.Scale(transform.forward, 
                dashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * drags.x + 1)) / -Time.deltaTime),
                0,
                (Mathf.Log(1f / (Time.deltaTime * drags.z + 1)) / -Time.deltaTime))
            );
        }

        // Process gravity
        _calcVelocity.y += gravity * Time.deltaTime;

        // Process dash ground drags
        _calcVelocity.x /= 1 + drags.x * Time.deltaTime;
        _calcVelocity.y /= 1 + drags.y * Time.deltaTime;
        _calcVelocity.z /= 1 + drags.z * Time.deltaTime;

        _characterController.Move(_calcVelocity * Time.deltaTime);
    }
    #endregion
}
