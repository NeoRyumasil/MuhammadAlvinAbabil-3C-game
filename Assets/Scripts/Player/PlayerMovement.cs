using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _walkSpeed;
    [SerializeField] private InputManager _input;

    // Start is called before the first frame update
    void Start()
    {
        _input.OnMoveInput += Move;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        _input.OnMoveInput -= Move;
    }

    private void Move(Vector2 axisDirection)
    {
        Vector3 movementDirection = new Vector3(axisDirection.x, 0, axisDirection.y);
        Debug.Log(movementDirection);
    }
}
