using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Player Attributes
    [Header("Player Attributes")]
    [SerializeField] private float _walkSpeed = 10f;
    [SerializeField] private float _sprintSpeed = 20f;
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _climbSpeed = 5f;
    [SerializeField] private float _crouchSpeed = 5f;
    private PlayerStance _playerStance;

    // Player Rotation
    [Header("Player Rotation")]
    [SerializeField] private float _rotationSmoothTime = 0.1f;
    [SerializeField] private float _rotationSmoothVelocity;

    // Player Sprint
    [Header("Player Sprint")]
    [SerializeField] private float _walkSprintTransition = 15f;

    // Player Jump
    [Header("Player Jump")]
    [SerializeField] private float _detectorRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayer;
    private bool _isGrounded;

    // Player Stair Climb
    [Header("Player Stair Climb")]
    [SerializeField] private Vector3 _upperStepOffset;
    [SerializeField] private float _stepCheckerDistance;
    [SerializeField] private float _stepForce;

    // Player Wall Climb
    [Header("Player Wall Climb")]
    [SerializeField] private float _climbCheckDistance;
    [SerializeField] private LayerMask _climbableLayer;
    [SerializeField] private Vector3 _climbOffset;

    // Game Object References
    [Header("Game Object References")]
    [SerializeField] private InputManager _input;
    [SerializeField] private Transform _groundDetector;
    [SerializeField] private Transform _climbDetector;
    [SerializeField] private Transform _leftClimbBorder;
    [SerializeField] private Transform _rightClimbBorder;
    [SerializeField] private Transform _upperClimbBorder;
    [SerializeField] private Transform _lowerClimbBorder;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private CameraManager _cameraManager;

    private Animator _animator;
    private CapsuleCollider _collider;

    // Components References
    private Rigidbody _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        // Input Manager
        _input.OnMoveInput += Move;
        _input.OnSprintInput += Sprint;
        _input.OnJumpInput += Jump;
        _input.OnClimbInput += StartClimb;
        _input.OnCancelClimbInput += CancelClimb;
        _input.OnCrouchInput += Crouch;

        // Camera Manager
        _cameraManager.OnChangePerspective += OnChangePerspective;
    }


    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Stance
        _playerStance = PlayerStance.Stand;

        // Game Object References
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<CapsuleCollider>();

        // Attributes
        _speed = _walkSpeed;

        // Methods
        HideAndLockCursor();
    }

    // Update is called once per frame
    void Update()
    {
        CheckIsGrounded();
        CheckStep();
    }

    // Menghapus event listener untuk menghindari memory leak
    private void OnDestroy()
    {
        // Input Manager
        _input.OnMoveInput -= Move;
        _input.OnSprintInput -= Sprint;
        _input.OnJumpInput -= Jump;
        _input.OnClimbInput -= StartClimb;
        _input.OnCancelClimbInput -= CancelClimb;
        _input.OnCrouchInput -= Crouch;

        // Camera Manager
        _cameraManager.OnChangePerspective -= OnChangePerspective;
    }

    // Pergerakan Player
    private void Move(Vector2 axisDirection)
    {
        Vector3 movementDirection = Vector3.zero;
        bool isPlayerStanding = _playerStance == PlayerStance.Stand;
        bool isPlayerClimbing = _playerStance == PlayerStance.Climb;
        bool isPlayerCrouching = _playerStance == PlayerStance.Crouch;

        // Pergerakan Player Berdiri
        if (isPlayerStanding || isPlayerCrouching)
        {

            // Animasi Player
            Vector3 velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
            _animator.SetFloat("Velocity", velocity.magnitude * axisDirection.magnitude * 0.15f);
            _animator.SetFloat("VelocityZ", axisDirection.y * velocity.magnitude * 0.15f);
            _animator.SetFloat("VelocityX", axisDirection.x * velocity.magnitude * 0.15f);

            // Setting TPS dan FPS Camera
            switch (_cameraManager.CameraState)
            {
            case CameraState.ThirdPerson:
                if (axisDirection.magnitude >= 0.1f)
                {
                    // Pergerakan Player
                    float rotationAngle = Mathf.Atan2(axisDirection.x, axisDirection.y) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;
                    float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotationAngle, ref _rotationSmoothVelocity, _rotationSmoothTime);
                    transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
                    movementDirection = Quaternion.Euler(0f, rotationAngle, 0f) * Vector3.forward;
                    _rigidbody.AddForce(movementDirection * _speed * Time.deltaTime, ForceMode.VelocityChange);
                }
                break;

            case CameraState.FirstPerson:
                transform.rotation = Quaternion.Euler(0f, _cameraTransform.eulerAngles.y, 0f);
                Vector3 verticalDirection = axisDirection.y * transform.forward;
                Vector3 horizontalDirection = axisDirection.x * transform.right;
                movementDirection = verticalDirection + horizontalDirection;
                _rigidbody.AddForce(movementDirection * _speed * Time.deltaTime, ForceMode.VelocityChange); 
                break;
                
            default:
                break;
            }
        }
        
        // Pergerakan Player Memanjat
        else if (isPlayerClimbing)
        {
            bool isInLeftClimbBorder = Physics.Raycast(_leftClimbBorder.position, transform.forward, out RaycastHit leftHit, _climbCheckDistance, _climbableLayer);
            bool isInRightClimbBorder = Physics.Raycast(_rightClimbBorder.position, transform.forward, out RaycastHit rightHit, _climbCheckDistance, _climbableLayer);
            bool isInUpperClimbBorder = Physics.Raycast(_upperClimbBorder.position, transform.forward, out RaycastHit upperHit, _climbCheckDistance, _climbableLayer);
            bool isInLowerClimbBorder = Physics.Raycast(_lowerClimbBorder.position, transform.forward, out RaycastHit lowerHit, _climbCheckDistance, _climbableLayer);
                
            // Border Climb Checker
            if (axisDirection.x < 0 && !isInLeftClimbBorder)
            {
                axisDirection.x = 0;
                _rigidbody.velocity = Vector3.zero;
            }

            if (axisDirection.x > 0 && !isInRightClimbBorder)
            {
                axisDirection.x = 0;
                _rigidbody.velocity = Vector3.zero;
            }

            if (axisDirection.y > 0 && !isInUpperClimbBorder)
            {
                axisDirection.y = 0;
                _rigidbody.velocity = Vector3.zero;
            }

            if (axisDirection.y < 0 && !isInLowerClimbBorder)
            {
                axisDirection.y = 0;
                _rigidbody.velocity = Vector3.zero;
            }

            // Pergerakan Player Memanjat
            Vector3 horizontal = axisDirection.x * transform.right;
            Vector3 vertical = axisDirection.y * transform.up;
            movementDirection = horizontal + vertical;
            _rigidbody.AddForce(movementDirection * _climbSpeed * Time.deltaTime, ForceMode.VelocityChange);

            // Animasi Player Memanjat
            Vector3 velocity = new Vector3(_rigidbody.velocity.x, _rigidbody.velocity.y, 0f);
            _animator.SetFloat("ClimbVelocityY", velocity.magnitude * axisDirection.y);
            _animator.SetFloat("ClimbVelocityX", velocity.magnitude * axisDirection.x);
        }
    }
    
    // Player Sprint
    private void Sprint(bool isSprint)
    {
        if (isSprint)
        {
            if (_speed < _sprintSpeed)
            {
                _speed = _speed + _walkSprintTransition * Time.deltaTime;
            }
        }
        else
        {
            if (_speed > _walkSpeed)
            {
                _speed = _speed - _walkSprintTransition * Time.deltaTime;
            }
        }
    }

    // Player Jump
    private void Jump()
    {
        if (_isGrounded)
        {
            Vector3 jumpDirection = Vector3.up;
            _rigidbody.AddForce(jumpDirection * _jumpForce, ForceMode.Impulse);
            _animator.SetTrigger("Jump");
        }
    }

    // Player Crouch
    private void Crouch()
    {
        if (_playerStance == PlayerStance.Stand)
        {
            // Set Stance and Speed
            _playerStance = PlayerStance.Crouch;
            _speed = _crouchSpeed;

            // Set Collider Height and Center
            _collider.height = 1.3f;
            _collider.center = Vector3.up * 0.66f;

            // Set Animasi Crouch
            _animator.SetBool("IsCrouch", true);
        }
        else if (_playerStance == PlayerStance.Crouch)
        {
            // Set Stance and Speed
            _playerStance = PlayerStance.Stand;
            _speed = _walkSpeed;

            // Set Collider Height and Center
            _collider.height = 1.8f;
            _collider.center = Vector3.up * 0.9f;

            // Set Animasi Crouch
            _animator.SetBool("IsCrouch", false);
        }
    }

    // Grounded Checker
    private void CheckIsGrounded()
    {
        _isGrounded = Physics.CheckSphere(_groundDetector.position, _detectorRadius, _groundLayer);
        _animator.SetBool("IsGrounded", _isGrounded);
    }

    // Stair Climb Step Checker
    private void CheckStep()
    {
        bool isHitLowerStep = Physics.Raycast(_groundDetector.position, transform.forward, _stepCheckerDistance);
        bool isHitUpperStep = Physics.Raycast(_groundDetector.position + _upperStepOffset, transform.forward, _stepCheckerDistance);

        if (isHitLowerStep && !isHitUpperStep)
        {
            _rigidbody.AddForce(0, _stepForce, 0);
        }
    }

    // Player Wall Climb
    private void StartClimb()
    {
        bool isInFrontOfClimbingWall = Physics.Raycast(_climbDetector.position, transform.forward, out RaycastHit hit, _climbCheckDistance ,_climbableLayer);
        bool isNotClimbing = _playerStance != PlayerStance.Climb;

        if (isInFrontOfClimbingWall && _isGrounded && isNotClimbing)
        {
            // Set FPS Clamped Camera
            _cameraManager.setFPSClampedCamera(true, transform.rotation.eulerAngles);

            // Set FOV TPS Camera
            _cameraManager.SetTPSFieldOfView(70f);

            // Set Collider
            _collider.center = Vector3.up * 1.3f;

            // Set Player Position and Rotation saat Memanjat
            Vector3 offset = (transform.forward * _climbOffset.z) + (Vector3.up * _climbOffset.y);
            transform.position = hit.point - offset;
            transform.rotation = Quaternion.LookRotation(-hit.normal, Vector3.up);
            _playerStance = PlayerStance.Climb;
            _rigidbody.useGravity = false;

            // Set Animasi Climb
            _animator.SetBool("IsClimb", true);
        }
    }

    // Player Cancel Climb
    private void CancelClimb()
    {
        if (_playerStance == PlayerStance.Climb)
        {
            // Set FPS Clamped Camera
            _cameraManager.setFPSClampedCamera(false, transform.rotation.eulerAngles);

            // Set FOV TPS Camera
            _cameraManager.SetTPSFieldOfView(40f);

            // Set Collider
            _collider.center = Vector3.up * 0.9f;

            // Set Player Stance dan Posisi saat Berhenti Memanjat
            _playerStance = PlayerStance.Stand;
            _rigidbody.useGravity = true;
            transform.position -= transform.forward * 1f;

            // Set Animasi Climb
            _animator.SetBool("IsClimb", false);
        }
    }

    // Menghapus dan mengunci kursor
    private void HideAndLockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Change Perspective 
    private void OnChangePerspective()
    {
        _animator.SetTrigger("ChangePerspective");
    }
    
    // Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // Climb Detector
        bool isInFrontOfClimbingWall = Physics.Raycast(_climbDetector.position, transform.forward, out RaycastHit hit, _climbCheckDistance, _climbableLayer);

        // Climb Border
        bool isInLeftClimbBorder = Physics.Raycast(_climbDetector.position, transform.forward, out RaycastHit leftHit, _climbCheckDistance, _climbableLayer);
        bool isInRightClimbBorder = Physics.Raycast(_climbDetector.position, transform.forward, out RaycastHit rightHit, _climbCheckDistance, _climbableLayer);
        bool isInUpperClimbBorder = Physics.Raycast(_climbDetector.position, transform.forward, out RaycastHit upperHit, _climbCheckDistance, _climbableLayer);
        bool isInLowerClimbBorder = Physics.Raycast(_climbDetector.position, transform.forward, out RaycastHit lowerHit, _climbCheckDistance, _climbableLayer);

        // Gizmos Condition Checker
        if (isInFrontOfClimbingWall)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_climbDetector.position, _climbDetector.position + (_climbDetector.forward * _climbCheckDistance));
        }

         if (isInLeftClimbBorder)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_leftClimbBorder.position, _leftClimbBorder.position + (_leftClimbBorder.forward * _climbCheckDistance));
        }

        if (isInRightClimbBorder)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_rightClimbBorder.position, _rightClimbBorder.position + (_rightClimbBorder.forward * _climbCheckDistance));
        }

        if (isInUpperClimbBorder)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_upperClimbBorder.position, _upperClimbBorder.position + (_upperClimbBorder.forward * _climbCheckDistance));
        }

        if (isInLowerClimbBorder)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_lowerClimbBorder.position, _lowerClimbBorder.position + (_lowerClimbBorder.forward * _climbCheckDistance));
        }
    }
}
