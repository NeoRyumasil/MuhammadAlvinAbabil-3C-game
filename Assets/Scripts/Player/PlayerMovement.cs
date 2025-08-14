using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Player Attributes
    [SerializeField] private float _walkSpeed = 350f;

    // Game Object References
    [SerializeField] private InputManager _input;

    // Components References
    private Rigidbody _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        _input.OnMoveInput += Move;
    }


    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Menghapus event listener untuk menghindari memory leak
    private void OnDestroy()
    {

        _input.OnMoveInput -= Move;
    }

    // Pergerakan Player
    private void Move(Vector2 axisDirection)
    {
        Vector3 movementDirection = new Vector3(axisDirection.x, 0, axisDirection.y);
        _rigidbody.AddForce(movementDirection * _walkSpeed * Time.deltaTime);
    }
}
