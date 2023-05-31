using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System/New Parkour Action")]

public class ParkourAction : ScriptableObject
{
    [field: SerializeField] public string AnimName { get; set; }
    [field: SerializeField] public string CurrentObstacleTag { get; set; }

    [field: SerializeField] public float MinHeight { get; set; }
    [field: SerializeField] public float MaxHeight { get; set; }
    [field: SerializeField] public bool RotateToObstacle { get; set; }

    [field: Tooltip("Length of the animation after the current animation before reaching the locomotion blend tree")]
    [field: SerializeField] public float PostActionDelay { get; set; }


    [field: Header("Target Matching")]
    [field: SerializeField] public bool IstargetMatchingEnabled { get; set; } = true;
    [field: SerializeField] public AvatarTarget MatchBodyPart { get; set; }
    [field: SerializeField] public float MatchStartTime { get; set; }
    [field: SerializeField] public float MatchTargetTime { get; set; }
    [field: SerializeField] public Vector3 MatchPosWeight { get; set; } = new Vector3(0,1,0);


    // Desired Rotation the player should be in to face the obstacle head on
    public Quaternion TargetRotation { get; set; }

    //Point on the obstacle where the body part should landing
    public Vector3 MatchPos { get;set; }    
    public bool Mirror { get; set; }

    public virtual bool CheckIfPossible(ObstacleHitData hitData, Transform player)
    {

        // Check Tag (for parkour no climbing)
        if(!string.IsNullOrEmpty(CurrentObstacleTag) && hitData.forwardHit.transform.tag != CurrentObstacleTag)
            return false;

        if (RotateToObstacle)
            TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal);

        if (IstargetMatchingEnabled)
            MatchPos = hitData.heightHit.point;

        // Dont check height for tagged animation return here itself
        if (hitData.forwardHit.transform.tag == CurrentObstacleTag)
        {
            return true;
        }

        // Getting the height of obstacle infront of the player
        float obstacleHeight = hitData.heightHit.point.y - player.position.y;
        if (obstacleHeight < MinHeight|| obstacleHeight>MaxHeight) return false;

        

        return true;
    }


}
