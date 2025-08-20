using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Player Attributes
    [SerializeField] private float _walkSpeed = 350f;
    [SerializeField] private float _sprintSpeed = 450f;
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce = 21000f;
    [SerializeField] private float _climbSpeed = 200f;
    private PlayerStance _playerStance;

    // Player Rotation
    [SerializeField] private float _rotationSmoothTime = 0.1f;
    [SerializeField] private float _rotationSmoothVelocity;

    // Player Sprint
    [SerializeField] private float _walkSprintTransition = 30f;

    // Player Jump
    [SerializeField] private float _detectorRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayer;
    private bool _isGrounded;

    // Player Stair Climb
    [SerializeField] private Vector3 _upperStepOffset;
    [SerializeField] private float _stepCheckerDistance;
    [SerializeField] private float _stepForce;

    // Player Wall Climb
    [SerializeField] private float _climbCheckDistance;
    [SerializeField] private LayerMask _climbableLayer;
    [SerializeField] private Vector3 _climbOffset;

    // Game Object References
    [SerializeField] private InputManager _input;
    [SerializeField] private Transform _groundDetector;
    [SerializeField] private Transform _climbDetector;

    // Components References
    private Rigidbody _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        _input.OnMoveInput += Move;
        _input.OnSprintInput += Sprint;
        _input.OnJumpInput += Jump;
        _input.OnClimbInput += StartClimb;
        _input.OnCancelClimbInput += CancelClimb;
    }


    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        _playerStance = PlayerStance.Stand;

        _rigidbody = GetComponent<Rigidbody>();
        _speed = _walkSpeed;
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
        _input.OnMoveInput -= Move;
        _input.OnSprintInput -= Sprint;
        _input.OnJumpInput -= Jump;
        _input.OnClimbInput -= StartClimb;
        _input.OnCancelClimbInput -= CancelClimb;
    }

    // Pergerakan Player
    private void Move(Vector2 axisDirection)
    {
        Vector3 movementDirection = Vector3.zero;
        bool isPlayerStanding = _playerStance == PlayerStance.Stand;
        bool isPlayerClimbing = _playerStance == PlayerStance.Climb;

        // Pergerakan Player
        if (axisDirection.magnitude > 0.1f)
        {
            // Pergerakan Player Berdiri
            if (isPlayerStanding)
            {
                // Rotasi Player
                float rotationAngle = Mathf.Atan2(axisDirection.x, axisDirection.y) * Mathf.Rad2Deg;
                float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotationAngle, ref _rotationSmoothVelocity, _rotationSmoothTime);
                transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
                movementDirection = Quaternion.Euler(0f, rotationAngle, 0f) * Vector3.forward;

                // Pergerakan Player
                _rigidbody.AddForce(movementDirection * _speed * Time.deltaTime, ForceMode.VelocityChange);
            }
            // Pergerakan Player Memanjat
            else if (isPlayerClimbing)
            {
                Vector3 horizontal = axisDirection.x * transform.right;
                Vector3 vertical = axisDirection.y * transform.up;
                movementDirection = horizontal + vertical;
                _rigidbody.AddForce(movementDirection * _climbSpeed * Time.deltaTime);
            }
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
        }
    }

    // Grounded Checker
    private void CheckIsGrounded()
    {
        _isGrounded = Physics.CheckSphere(_groundDetector.position, _detectorRadius, _groundLayer);
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
            Vector3 offset = (transform.forward * _climbOffset.z) + (Vector3.up * _climbOffset.y);
            transform.position = hit.point - offset;
            _playerStance = PlayerStance.Climb;
            _rigidbody.useGravity = false;
        }
    }

    // Player Cancel Climb
    private void CancelClimb()
    {
        if (_playerStance == PlayerStance.Climb)
        {
            _playerStance = PlayerStance.Stand;
            _rigidbody.useGravity = true;
            transform.position -= transform.forward * 1f;
        }
    }

    private void OnDrawGizmos()
    {
        // Climb Detector
        Gizmos.color = Color.red;
        bool isInFrontOfClimbingWall = Physics.Raycast(_climbDetector.position, transform.forward, out RaycastHit hit, _climbCheckDistance, _climbableLayer);

        if (isInFrontOfClimbingWall)
        {
            Gizmos.color = Color.green;
        }

        Gizmos.DrawLine(_climbDetector.position, _climbDetector.position + (_climbDetector.forward * _climbCheckDistance));
        
    }
}
