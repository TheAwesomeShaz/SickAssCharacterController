using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System / New Parkour Action")]

public class ParkourAction : ScriptableObject
{
    [field: SerializeField] public string AnimName { get; set; }
    [field: SerializeField] public float MinHeight { get; set; }
    [field: SerializeField] public float MaxHeight { get; set; }
    [field: SerializeField] public bool RotateToObstacle { get; set; }

    [field: Header("Target Matching")]
    [field: SerializeField] public bool IstargetMatchingEnabled { get; set; } = true;
    [field: SerializeField] public AvatarTarget MatchBodyPart { get; set; }
    [field: SerializeField] public float MatchStartTime { get; set; }
    [field: SerializeField] public float MatchTargetTime { get; set; }

    // Desired Rotation the player should be in to face the obstacle head on
    public Quaternion TargetRotation { get; set; }

    //Point on the obstacle where the body part should landing
    public Vector3 MatchPos { get;set; }    

    public bool CheckIfPossible(ObstacleHitData hitData, Transform player)
    {
        // Getting the height of obstacle infront of the player
        float obstacleHeight = hitData.heightHit.point.y - player.position.y;
        if (obstacleHeight < MinHeight|| obstacleHeight>MaxHeight) return false;

        if (RotateToObstacle)
            TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal);

        if (IstargetMatchingEnabled)
            MatchPos = hitData.heightHit.point;

        return true;
    }


}
