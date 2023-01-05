using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControls playerControls;
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

    private void Awake()
    {
        animationManager = GetComponent<AnimatorManager>();    
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    private void OnEnable()
    {
        if(playerControls == null) 
        { 
            playerControls = new PlayerControls();
            playerControls.PlayerMovement.Movement.performed += ctx =>
            {
                movementInput = ctx.ReadValue<Vector2>();
            };
            playerControls.PlayerMovement.Camera.performed += ctx =>
            {
                cameraInput = ctx.ReadValue<Vector2>();
            };
            playerControls.PlayerActions.HighProfileModifier.performed += ctx => highProfileInput = true;
            playerControls.PlayerActions.HighProfileModifier.canceled += ctx => highProfileInput = false;
            playerControls.PlayerActions.Jump.performed += ctx => jumpInput = true; 
        }
        playerControls.Enable();    
    }
   
    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        HandleJumpingInput();
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

    void HandleJumpingInput()
    {
        if (jumpInput)
        {
            jumpInput = false;
            playerLocomotion.HandleJumping();
        }
    }
}
