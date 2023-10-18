using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LandingBehaviour : StateMachineBehaviour
{
    PlayerStateMachine playerStateMachine;
    [SerializeField] float setControlFalseDelay = 0.2f;
    [SerializeField] float setControlTrueDelay = 0.2f;
    bool hasSetControlFalse;
    bool hasSetControlTrue;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(playerStateMachine == null) { playerStateMachine = animator.GetComponent<PlayerStateMachine>(); }

        hasSetControlFalse = false;
        hasSetControlTrue = false;

        if(setControlFalseDelay == 0) {
            //Debug.Log("Set Control False for normal Land anim");
            playerStateMachine.SetControl(false);
            hasSetControlFalse = true;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!hasSetControlFalse && stateInfo.normalizedTime >= setControlFalseDelay)
        {
            playerStateMachine.SetControl(false);
            //Debug.Log("SetControl is False");
            hasSetControlFalse = true;
        }
        
        if (!hasSetControlTrue && stateInfo.normalizedTime >= setControlTrueDelay)
        {
            playerStateMachine.SetControl(true);
            //Debug.Log("Set Control is True");
            hasSetControlTrue = true;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("FallRoll"))
        {
            // TODO : Set Speed to Run speed in a non gae wae later
            //playerStateMachine.SetSpeedToRunning();
        }
        playerStateMachine.SetControl(true);

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
