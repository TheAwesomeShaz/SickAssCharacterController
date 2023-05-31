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

        animatorManager.OnSetInteractingOrLedge += (isInteracting,isOnLedge) => {
            IsInteracting = isInteracting;
            playerLocomotion.IsOnLedge = isOnLedge;
            playerLocomotion.SetControl(!IsInteracting);
        };

    }

    private void Update()
    {

        inputManager.HandleAllInputs();

        //TODO: Change jump input to high profile modifier later
        // when it starts making a bit more sense
        animatorManager.HandleAllParkour(inputManager.jumpInput,IsInteracting, playerLocomotion.IsOnLedge);       
        
        playerLocomotion.HandleAllMovement(inputManager.movementInput,IsInteracting);
    }


    private void LateUpdate()
    {
        cameraManager.HandleAllCameraMovement();
        
        //playerLocomotion.isJumping = animator.GetBool("isJumping");
        //animator.SetBool("isGrounded", playerLocomotion.isGrounded);
    }
}
