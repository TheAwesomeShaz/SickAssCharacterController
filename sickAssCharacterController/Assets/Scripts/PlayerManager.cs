using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    Animator animator;
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;
    CameraManager cameraManager;
    ParkourController parkourController;

    public bool IsInteracting;

    private void Awake()
    {
        cameraManager = FindObjectOfType<CameraManager>();
        animator = GetComponent<Animator>();
        inputManager = GetComponent<  InputManager>();    
        playerLocomotion = GetComponent<PlayerLocomotion>();    
        parkourController= GetComponent<ParkourController>();

        parkourController.OnSetInteracting += (boolValue) => {
            Debug.Log("Bool value " + boolValue);
            IsInteracting = boolValue;
            playerLocomotion.SetControl(!boolValue);
            Debug.Log("Is Interacting " + IsInteracting);
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
        
        IsInteracting = animator.GetBool("isInteracting");
        //playerLocomotion.isJumping = animator.GetBool("isJumping");
        //animator.SetBool("isGrounded", playerLocomotion.isGrounded);
    }
}
