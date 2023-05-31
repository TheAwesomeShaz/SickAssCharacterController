using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    [SerializeField] List<ParkourAction> parkoutActions;    

    EnvironmentScanner environmentScanner;
    AnimatorManager animatorManager;
    Animator animator;
    [SerializeField] float rotateTowardsObstacleSpeed = 500f;
    bool isInteracting;

    public event Action<bool> OnSetInteracting;

    private void Awake()
    {
        environmentScanner = GetComponent<EnvironmentScanner>();
        animatorManager = GetComponent<AnimatorManager>();
        animator = GetComponent<Animator>();
    }

    public void HandleAllParkour(bool jumpInput, bool isInteracting)
    {
        HandleObstacleCheck(jumpInput, isInteracting);
    }

    public void HandleObstacleCheck(bool jumpInput, bool isInteracting)
    {

        if (jumpInput && !isInteracting)
        {
            var hitData = environmentScanner.ObstacleCheck();
            if (hitData.forwardHitFound)
            {
                foreach (var action in parkoutActions)
                {
                    if (action.CheckIfPossible(hitData,transform))
                    {
                        StartCoroutine(DoParkourAction(action));
                        break;
                    }
                }

            }
        }
    }

    IEnumerator DoParkourAction(ParkourAction action)
    {
        isInteracting = true;
        OnSetInteracting?.Invoke(isInteracting);

        animatorManager.PlayTargetAnimation(action.AnimName);

        animator.SetBool("MirrorAction", action.Mirror);

        // we return null to wait for this frame to end before fetching the animator State
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(0);
        if (!animState.IsName(action.AnimName))
            Debug.LogError("Parkour Animation Name is spelled wrong");

        // wait for the current parkour animation to complete
        // The loop below means that while "the action is being performed" do this 
        float timer = 0f;
        while (timer <= animState.length)
        {
            timer += Time.deltaTime;
            
            if(action.RotateToObstacle) 
                transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                    action.TargetRotation, rotateTowardsObstacleSpeed* Time.deltaTime);

            if (action.IstargetMatchingEnabled)
                MatchTarget(action);

            if(animator.IsInTransition(0) && timer >= 0.5f)
                break;

            // return null makes it do nothing i.e it makes it wait till end of while loop?
            yield return null;
        }

        yield return new WaitForSeconds(action.PostActionDelay);

        isInteracting = false;
        OnSetInteracting?.Invoke(isInteracting);
    }

    void MatchTarget(ParkourAction action)
    {
        if (animator.isMatchingTarget) return;

        animator.MatchTarget(action.MatchPos,transform.rotation,
            action.MatchBodyPart,new MatchTargetWeightMask(action.MatchPosWeight,0),
            action.MatchStartTime,action.MatchTargetTime);
    }

}
