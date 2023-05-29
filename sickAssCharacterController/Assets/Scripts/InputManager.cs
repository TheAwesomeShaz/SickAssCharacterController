using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour,PlayerControls.IPlayerMovementActions,PlayerControls.IPlayerActionsActions
{
    PlayerLocomotion playerLocomotion;
    AnimatorManager animationManager;

    public Vector2 movementInput;
    public Vector2 cameraInput;
        
    public float verticalInput;
    public float horizontalInput;
    public float cameraInputX;
    public float cameraInputY;

    public float moveAmount; // the amount the left analog joystick has moved btwn 0 to 1
    public bool highProfileInput;
    public bool jumpInput;

    PlayerControls playerControls;

    private void Awake()
    {
        animationManager = GetComponent<AnimatorManager>();    
        playerLocomotion = GetComponent<PlayerLocomotion>();
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
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
        // Right Joystick Input (or Mouse Delta)
        cameraInputX = cameraInput.x;
        cameraInputY = cameraInput.y;

        // DOUBT: we are clamping the move amount between zero and 1 due to the blend tree stuff in the animator it can only take values from zero to one due to some reason? will probably fix later
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animationManager.UpdateAnimatorValues(0, moveAmount, playerLocomotion.isSprinting);
    }

    void HandleSprintingInput()
    {
        if (highProfileInput && moveAmount > 0.5f)
        {
            playerLocomotion.isSprinting = true;
        }
        else
        {
            playerLocomotion.isSprinting = false;
        }
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnCamera(InputAction.CallbackContext context)
    {
        cameraInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) { jumpInput = true; }
        if (context.canceled) { jumpInput = false; }
    }

    public void OnHighProfileModifier(InputAction.CallbackContext context)
    {
        if(context.performed) highProfileInput = true;
        if (context.canceled) highProfileInput = false;
    }
}
