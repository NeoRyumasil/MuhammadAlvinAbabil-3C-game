using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    // Event Listener untuk menangani input pergerakan
    public Action<Vector2> OnMoveInput;
    public Action<bool> OnSprintInput;
    public Action OnJumpInput;
    public Action OnClimbInput;
    public Action OnCancelClimbInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckMovementInput();
        CheckSprintInput();
        CheckJumpInput();
        CheckCrouchInput();
        CheckChangePOVInput();
        CheckClimbInput();
        CheckGlideInput();
        CheckCancelInput();
        CheckPunchInput();
        CheckMainMenuInput();
    }

    // Check Input Pergerakan
    private void CheckMovementInput()
    {
        float verticalAxis = Input.GetAxis("Vertical");
        float horizontalAxis = Input.GetAxis("Horizontal");
        Vector2 inputAxis = new Vector2(horizontalAxis, verticalAxis);

        if (OnMoveInput != null)
        {
            OnMoveInput(inputAxis);
        }
    }

    // Check Input Sprint
    private void CheckSprintInput()
    {
        bool isHoldSprintInput = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (isHoldSprintInput)
        {
            if (OnSprintInput != null)
            {
                OnSprintInput(true);
            }
        }
        else
        {
            if (OnSprintInput != null)
            {
                OnSprintInput(false);
            }
        }
    }

    // Check Input Jump
    private void CheckJumpInput()
    {
        bool isPressJumpInput = Input.GetKeyDown(KeyCode.Space);

        if (isPressJumpInput)
        {
            if (OnJumpInput != null)
            {
                OnJumpInput();
            }
        }
    }

    // Check Input Crouch
    private void CheckCrouchInput()
    {
        bool isPressCrouchInput = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl);

        if (isPressCrouchInput)
        {
            Debug.Log("Crouch");
        }
    }

    // Check Input Change POV Camera
    private void CheckChangePOVInput()
    {
        bool isPressChangePOVInput = Input.GetKeyDown(KeyCode.Q);

        if (isPressChangePOVInput)
        {
            Debug.Log("Change POV");
        }
    }

    // Check Input Climb
    private void CheckClimbInput()
    {
        bool isPressClimbInput = Input.GetKeyDown(KeyCode.E);

        if (isPressClimbInput)
        {
            OnClimbInput();
        }
    }

    // Check Input Glide
    private void CheckGlideInput()
    {
        bool isPressGlideInput = Input.GetKeyDown(KeyCode.G);

        if (isPressGlideInput)
        {
            Debug.Log("Glide");
        }
    }

    // Check Input Cancel
    private void CheckCancelInput()
    {
        bool isPressCancelInput = Input.GetKeyDown(KeyCode.C);

        if (isPressCancelInput)
        {
            if (OnCancelClimbInput != null)
            {
                OnCancelClimbInput();
            }
        }
    }

    // Check Input Punch
    private void CheckPunchInput()
    {
        bool isPressPunchInput = Input.GetKeyDown(KeyCode.Mouse0);

        if (isPressPunchInput)
        {
            Debug.Log("Punch");
        }
    }

    // Check Input Main Menu
    private void CheckMainMenuInput()
    {
        bool isPressMainMenuInput = Input.GetKeyDown(KeyCode.Escape);

        if (isPressMainMenuInput)
        {
            Debug.Log("Back To Main Menu");
        }
    }
}
