using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;
    CameraManager cameraManager;
    ParkourController parkourController;

    bool IsInteracting;

    private void Awake()
    {
        cameraManager = FindObjectOfType<CameraManager>();
        inputManager = GetComponent<  InputManager>();    
        playerLocomotion = GetComponent<PlayerLocomotion>();    
        parkourController= GetComponent<ParkourController>();

        parkourController.OnSetInteracting += (boolValue) => {
            IsInteracting = boolValue;
            playerLocomotion.SetControl(!IsInteracting);
        };
    }

    private void Update()
    {

        inputManager.HandleAllInputs();

        //TODO: Change jump input to high profile modifier later
        // when it starts making a bit more sense
        parkourController.HandleAllParkour(inputManager.jumpInput,IsInteracting);       
        
        playerLocomotion.HandleAllMovement(inputManager.movementInput,IsInteracting);
    }


    private void LateUpdate()
    {
        cameraManager.HandleAllCameraMovement();
        
        //playerLocomotion.isJumping = animator.GetBool("isJumping");
        //animator.SetBool("isGrounded", playerLocomotion.isGrounded);
    }
}
