using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    EnvironmentScanner environmentScanner;
    AnimatorManager animatorManager;

    readonly int StepUpHash = Animator.StringToHash("StepUp");
    bool isInteracting;

    public event Action<bool> OnSetInteracting;

    private void Awake()
    {
        environmentScanner = GetComponent<EnvironmentScanner>();
        animatorManager = GetComponent<AnimatorManager>();
    }

    public void HandleAllParkour(bool jumpInput, bool isInteracting)
    {
        HandleObstacleCheck(jumpInput, isInteracting);
    }

    public void HandleObstacleCheck(bool jumpInput, bool isInteracting)
    {

        if (jumpInput && !isInteracting)
        {
            Debug.Log("Inside handleObstacle");
            var hitData = environmentScanner.ObstacleCheck();
            if (hitData.forwardHitFound)
            {
                StartCoroutine(DoParkourAction());
            }
        }
    }

    IEnumerator DoParkourAction()
    {
        isInteracting = true;
        OnSetInteracting?.Invoke(isInteracting);

        animatorManager.PlayTargetAnimation(StepUpHash, isInteracting);

        // we return null to wait for this frame to end before fetching the animator State
        yield return null;

        var animState = animatorManager.animator.GetNextAnimatorStateInfo(0);

        // wait for the current parkour animation to complete
        yield return new WaitForSeconds(animState.length);

        isInteracting = false;
        OnSetInteracting?.Invoke(isInteracting);

    }

}
