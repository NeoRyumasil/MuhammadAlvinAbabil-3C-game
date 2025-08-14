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

    // Player Rotation
    [SerializeField] private float _rotationSmoothTime = 0.1f;
    [SerializeField] private float _rotationSmoothVelocity;

    // Player Sprint
    [SerializeField] private float _walkSprintTransition = 30f;

    // Player Jump
    [SerializeField] private float _detectorRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayer;
    private bool _isGrounded;

    // Game Object References
    [SerializeField] private InputManager _input;
    [SerializeField] private Transform _groundDetector;

    // Components References
    private Rigidbody _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        _input.OnMoveInput += Move;
        _input.OnSprintInput += Sprint;
    }


    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        _input.OnJumpInput += Jump;

        _rigidbody = GetComponent<Rigidbody>();
        _speed = _walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        CheckIsGrounded();
    }

    // Menghapus event listener untuk menghindari memory leak
    private void OnDestroy()
    {

        _input.OnMoveInput -= Move;
        _input.OnSprintInput -= Sprint;
        _input.OnJumpInput -= Jump;
    }

    // Pergerakan Player
    private void Move(Vector2 axisDirection)
    {
        Vector3 movementDirection = new Vector3(axisDirection.x, 0, axisDirection.y);
        _rigidbody.AddForce(movementDirection * _walkSpeed * Time.deltaTime);

        // Rotasi Player
        if (axisDirection.magnitude > 0.1f)
        {
            float rotationAngle = Mathf.Atan2(axisDirection.x, axisDirection.y) * Mathf.Rad2Deg;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotationAngle, ref _rotationSmoothVelocity, _rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
            movementDirection = Quaternion.Euler(0f, rotationAngle, 0f) * Vector3.forward;
            _rigidbody.AddForce(movementDirection * Time.deltaTime * _walkSpeed);
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
            _rigidbody.AddForce(jumpDirection * _jumpForce * Time.deltaTime);
        }
    }

    // Grounded Checker
    private void CheckIsGrounded()
    {
        _isGrounded = Physics.CheckSphere(_groundDetector.position, _detectorRadius, _groundLayer);
    }
}
