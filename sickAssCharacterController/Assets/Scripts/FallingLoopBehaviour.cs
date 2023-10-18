using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FallingLoopBehaviour : StateMachineBehaviour
{
    PlayerStateMachine playerStateMachine;
    [SerializeField] float groundedCheckDelay = 0.5f;
    bool hasTransitioned;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hasTransitioned = false;

        if (playerStateMachine == null) { playerStateMachine = animator.GetComponent<PlayerStateMachine>(); }
        playerStateMachine.SetControl(true);

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (!hasTransitioned && stateInfo.normalizedTime >= groundedCheckDelay && playerStateMachine.IsGrounded)
        {
            if (stateInfo.IsName("FallForward"))
                animator.CrossFade("FallRoll", 0.2f);

            if (stateInfo.IsName("Fall"))
                animator.CrossFadeInFixedTime("Land", 0.2f);

            groundedCheckDelay = 0.2f;
            hasTransitioned = true;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //}

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
