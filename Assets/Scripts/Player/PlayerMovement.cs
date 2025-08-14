using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Player Attributes
    [SerializeField] private float _walkSpeed = 350f;
    [SerializeField] private float _sprintSpeed = 450f;
    [SerializeField] private float _speed;

    // Player Rotation
    [SerializeField] private float _rotationSmoothTime = 0.1f;
    [SerializeField] private float _rotationSmoothVelocity;

    // Player Sprint
    [SerializeField] private float _walkSprintTransition = 30f;

    // Game Object References
    [SerializeField] private InputManager _input;

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
        _rigidbody = GetComponent<Rigidbody>();
        _speed = _walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Menghapus event listener untuk menghindari memory leak
    private void OnDestroy()
    {

        _input.OnMoveInput -= Move;
        _input.OnSprintInput -= Sprint;
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
            _speed = _speed + _walkSprintTransition * Time.deltaTime;
        }
        else
        {
            if (_speed > _walkSpeed)
            {
                _speed = _speed - _walkSprintTransition * Time.deltaTime;
            }
        }
    }
}
