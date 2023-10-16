using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;
    CameraManager cameraManager;
    AnimatorManager animatorManager;

    bool IsInteracting;

    private void Awake()
    {
        cameraManager = FindObjectOfType<CameraManager>();
        inputManager = GetComponent<InputManager>();    
        playerLocomotion = GetComponent<PlayerLocomotion>();    
        animatorManager = GetComponent<AnimatorManager>();

        animatorManager.OnSetInteracting += (isInteracting) =>
        {
            IsInteracting = isInteracting;
            playerLocomotion.SetControl(!IsInteracting);
        };

    }

    private void Update()
    {

        inputManager.HandleAllInputs();

        // TODO: Change jump input to high profile modifier later
        // when the controls starts making a bit more sense
        playerLocomotion.HandleAllMovement(inputManager.MovementInput,IsInteracting,inputManager.HighProfileInput);

        animatorManager.HandleAllParkour(inputManager.JumpInput,IsInteracting, 
            playerLocomotion.IsOnLedge, playerLocomotion.LedgeHitData,inputManager.HighProfileInput);       

        // Not Doing Climbing right now, will continue later if required currently shifted to making ladder system
        //animatorManager.HandleAllClimbing(inputManager.movementInput,inputManager.jumpInput,playerLocomotion.IsHanging,IsInteracting);

    }


    private void LateUpdate()
    {
        // This is old code
        //cameraManager.HandleAllCameraMovement();
        
        //playerLocomotion.isJumping = animator.GetBool("isJumping");
        //animator.SetBool("isGrounded", playerLocomotion.isGrounded);
    }
}
