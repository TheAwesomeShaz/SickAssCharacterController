using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LandingBehaviour : StateMachineBehaviour
{
    PlayerLocomotion playerLocomotion;
    [SerializeField] float setControlFalseDelay = 0.2f;
    [SerializeField] float setControlTrueDelay = 0.2f;
    bool hasSetControlFalse;
    bool hasSetControlTrue;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(playerLocomotion == null) { playerLocomotion = animator.GetComponent<PlayerLocomotion>(); }

        hasSetControlFalse = false;
        hasSetControlTrue = false;

        if(setControlFalseDelay == 0) {
            Debug.Log("Set Control False for normal Land anim");
            playerLocomotion.SetControl(false);
            hasSetControlFalse = true;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!hasSetControlFalse && stateInfo.normalizedTime >= setControlFalseDelay)
        {
            playerLocomotion.SetControl(false);
            Debug.Log("SetControl is False");
            hasSetControlFalse = true;
        }
        
        if (!hasSetControlTrue && stateInfo.normalizedTime >= setControlTrueDelay)
        {
            playerLocomotion.SetControl(true);
            Debug.Log("SetControl is True");
            hasSetControlTrue = true;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("FallRoll"))
        {
            playerLocomotion.SetSpeedToRunning();
        }
        playerLocomotion.SetControl(true);

    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
