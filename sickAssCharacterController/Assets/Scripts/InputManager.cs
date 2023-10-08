using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour,PlayerControls.IPlayerMovementActions,PlayerControls.IPlayerActionsActions
{
    [SerializeField] private PlayerStateMachine playerStateMachine;
    //AnimatorManager animatorManager;

    public Vector2 MovementInput { get; set; }
    public Vector2 CameraInput { get; set; }

    public float VerticalInput{ get; set; }
    public float HorizontalInput{ get; set; }
    public float CameraInputX{ get; set;}
    public float CameraInputY{ get; set;}

    public float MoveAmount { get; private set; } // the amount the left analog joystick has moved btwn 0 to 1
    public bool HighProfileInput { get; private set; }
    public bool JumpInput { get; private set; }

    PlayerControls playerControls;

    private void Awake()
    {
        //animatorManager = GetComponent<AnimatorManager>();
        playerStateMachine = GetComponent<PlayerStateMachine>();
    }

    void Start()
    {
        playerControls = new PlayerControls();
        playerControls.PlayerMovement.SetCallbacks(this);
        playerControls.PlayerActions.SetCallbacks(this);
        playerControls.PlayerMovement.Enable();
        playerControls.PlayerActions.Enable();
    }

    void OnDestroy()
    {
        playerControls.PlayerMovement.Disable();
        playerControls.PlayerActions.Disable();
    }
   
    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleSprintingInput();
    }
    
    void HandleMovementInput()
    {
        // Left Joystick Input (or WASD)
        VerticalInput = MovementInput.y;
        HorizontalInput = MovementInput.x;
        // Right Joystick Input (or Mouse Delta)
        CameraInputX = CameraInput.x;
        CameraInputY = CameraInput.y;

        // DOUBT: we are clamping the move amount between zero and 1 due to the blend tree stuff
        // EXPLAINATION: blend tree only takes values from 0 to 1 so we normalize in a way
        //moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        //animatorManager.UpdateAnimatorValues(0, moveAmount, playerLocomotion.isSprinting);

        // We are not using the above code, we are just setting the moveamount according to the velocity of the player
        // from the playerLocomotion script
    }

    // TODO: this thing also should go in a State?
    void HandleSprintingInput()
    {
        if (HighProfileInput && MoveAmount > 0.5f)
        {
            playerStateMachine.IsSprinting = true;
        }
        else
        {
            playerStateMachine.IsSprinting = false;
        }
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    public void OnCamera(InputAction.CallbackContext context)
    {
        CameraInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) { JumpInput = true; }
        if (context.canceled) { JumpInput = false; }
    }

    public void OnHighProfileModifier(InputAction.CallbackContext context)
    {
        if(context.performed) HighProfileInput = true;
        if (context.canceled) HighProfileInput = false;
    }
}
