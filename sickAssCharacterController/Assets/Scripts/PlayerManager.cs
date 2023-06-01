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
        //Time.timeScale = 0.5f;

        cameraManager = FindObjectOfType<CameraManager>();
        inputManager = GetComponent<InputManager>();    
        playerLocomotion = GetComponent<PlayerLocomotion>();    
        animatorManager = GetComponent<AnimatorManager>();

        animatorManager.OnSetInteracting += (isInteracting) => {
            IsInteracting = isInteracting;
            playerLocomotion.SetControl(!IsInteracting);
        };

    }

    private void Update()
    {

        inputManager.HandleAllInputs();

        //TODO: Change jump input to high profile modifier later
        // when it starts making a bit more sense
        animatorManager.HandleAllParkour(inputManager.jumpInput,IsInteracting, playerLocomotion.IsOnLedge, playerLocomotion.LedgeHitData);       
        
        playerLocomotion.HandleAllMovement(inputManager.movementInput,IsInteracting);
    }


    private void LateUpdate()
    {
        cameraManager.HandleAllCameraMovement();
        
        //playerLocomotion.isJumping = animator.GetBool("isJumping");
        //animator.SetBool("isGrounded", playerLocomotion.isGrounded);
    }
}
