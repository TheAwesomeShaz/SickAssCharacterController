using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System/Custom Actions/New Vault Action")]
public class VaultAction : ParkourAction
{
    public override bool CheckIfPossible(ObstacleHitData hitData, Transform player)
    {
        if(!base.CheckIfPossible(hitData, player))
            return false;

        var hitPoint = hitData.forwardHit.transform.InverseTransformPoint(hitData.forwardHit.point);

        // if we are approaching obstacle from back and it is on our left or
        // if we are approaching obstacle from front and it is on our right 
        if (hitPoint.z < 0 && hitPoint.x < 0 || hitPoint.z > 0 && hitPoint.x > 0)
        {
            //Mirror Animation
            Mirror = true;
            MatchBodyPart = AvatarTarget.RightHand;
        }
        else
        {
            Mirror = false;
            MatchBodyPart = AvatarTarget.LeftHand;
        }

        // to indicate that the Action is possible
        return true;
    }
}
