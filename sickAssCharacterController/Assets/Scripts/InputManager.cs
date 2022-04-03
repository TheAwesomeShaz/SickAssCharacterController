using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControls playerControls;
    AnimationManager animationManager;

    [SerializeField] Vector2 movementInput;

    float moveAmount;
    public float verticalInput;
    public float horizontalInput;

    private void Awake()
    {
        animationManager = GetComponent<AnimationManager>();    
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
        }
        playerControls.Enable();    
    }
   
    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
    }

    void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animationManager.UpdateAnimatorValues(0, moveAmount);
    }
}
