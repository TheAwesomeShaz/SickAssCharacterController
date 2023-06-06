using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class ClimbController : MonoBehaviour
{
    EnvironmentScanner envScanner;
    AnimatorManager animatorManager;

    private void Awake()
    {
        envScanner = GetComponent<EnvironmentScanner>();
        animatorManager = GetComponent<AnimatorManager>();  
    }

    public void HandleAllClimbing(bool jumpInput)
    {
        if(jumpInput)
        {
            if(envScanner.ClimbLedgeCheck(transform.forward, out RaycastHit climbLedgeHit))
            {
                Debug.Log("Can Climb this one ledge infront of me");
            }
        }
    }

    //TODO continue making JumpToLedge
    //IEnumerator JumpToLedge(string anim, Transform ledge, float matchStartTime, float matchTargetTime)
    //{
    //    var matchParams = new MatchTargetParams()
    //    {
    //        pos = ledge.position,
    //        bodyPart = AvatarTarget.RightHand,
    //        startTime = matchStartTime,
    //        targetTime = matchTargetTime,
    //        posWeight = Vector3.one,
    //    };

    //    animatorManager.DoParkourAction(anim,matchParams,)
    //}

}
