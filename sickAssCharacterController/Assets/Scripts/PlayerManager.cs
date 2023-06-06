using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;
    CameraManager cameraManager;
    AnimatorManager animatorManager;
    ClimbController climbController;

    bool IsInteracting;

    private void Awake()
    {
        cameraManager = FindObjectOfType<CameraManager>();
        inputManager = GetComponent<InputManager>();    
        playerLocomotion = GetComponent<PlayerLocomotion>();    
        animatorManager = GetComponent<AnimatorManager>();
        climbController = GetComponent<ClimbController>();

        animatorManager.OnSetInteracting += (isInteracting) => {
            IsInteracting = isInteracting;
            playerLocomotion.SetControl(!IsInteracting);
        };

    }

    private void Update()
    {

        inputManager.HandleAllInputs();

        // TODO: Change jump input to high profile modifier later
        // when the controls starts making a bit more sense

        animatorManager.HandleAllParkour(inputManager.jumpInput,IsInteracting, 
            playerLocomotion.IsOnLedge, playerLocomotion.LedgeHitData,inputManager.highProfileInput);       
        
        playerLocomotion.HandleAllMovement(inputManager.movementInput,IsInteracting,inputManager.highProfileInput);

        climbController.HandleAllClimbing(inputManager.jumpInput);

    }


    private void LateUpdate()
    {
        cameraManager.HandleAllCameraMovement();
        
        //playerLocomotion.isJumping = animator.GetBool("isJumping");
        //animator.SetBool("isGrounded", playerLocomotion.isGrounded);
    }
}
