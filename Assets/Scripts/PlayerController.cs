using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _playerSpeed = 10f;
    [SerializeField] private float _jumpForce = 5f;

    private float verticalVelocity;

    private InputAction _jumpAction;
    private InputAction _moveAction;
    [SerializeField]private Vector2 _moveInput;

    private CharacterController _controller;
    private Animator _animator;
    
    [SerializeField] private Transform _sensorPosition;
    [SerializeField] private float _sensorRadius;
    [SerializeField] private LayerMask _groundLayer;

    [SerializeField] private float _gravity = -9.81f;
    private Vector3 _playerGravity;
    [SerializeField] private Transform cam;
    [SerializeField] private float TurnSmoothTime = 0.1f;
    [SerializeField] private float _smoothVelocity;


    void Awake()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();

        _jumpAction = InputSystem.actions["Jump"];
        _moveAction = InputSystem.actions["Move"];
    }

    void Update()
    {
    
        _moveInput = _moveAction.ReadValue<Vector2>();

        if(_jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            _animator.SetBool("IsJumping", true);
            Jump();
        }
         verticalVelocity += _gravity * Time.deltaTime;
         _controller.Move(new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);

        Movement();
        Gravity();
    }

    void Movement()
    {
        Vector3 _moveDirection = new Vector3(_moveInput.x, 0f, _moveInput.y);
        _animator.SetFloat("Horizontal", 0);
        _animator.SetFloat("Vertical", _moveDirection.magnitude);

        
        
        if (_moveDirection != Vector3.zero)
            {
            float targetAngle = Mathf.Atan2(_moveDirection.x, _moveDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref TurnSmoothTime, _smoothVelocity);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _controller.Move(_playerSpeed * Time.deltaTime * moveDirection);
            }
        
    }

   void Jump()
        {
            if (IsGrounded() && verticalVelocity < 0)
            {
                verticalVelocity = -1f; 
            }

            if (_jumpAction.WasPressedThisFrame() && IsGrounded())
            {
                verticalVelocity = _jumpForce;
            } 
            Debug.Log("Has Saltado");
        }
        
    

    void Gravity()
    {
        _playerGravity.y += -_gravity * Time.deltaTime;
        if(!IsGrounded() && _gravity < 0)
        {
            _controller.Move(_playerGravity * -2f * Time.deltaTime);
        }
        else if(IsGrounded())
        {
            _playerGravity.y = -9.81f;
            _animator.SetBool("IsJumping", false);
        }
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(_sensorPosition.position, _sensorRadius, _groundLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_sensorPosition.position, _sensorRadius);
    }
}
