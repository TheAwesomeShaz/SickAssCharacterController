using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnvironmentScanner : MonoBehaviour
{
    [Header("Obstacle Detection")]
    [SerializeField] private Vector3 forwardRayOffset = new Vector3(0f, 0.25f, 0f);
    [SerializeField] private float forwardRayLength = 1f;
    [SerializeField] private float heightRayLength = 5f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Edge Detection")]
    [SerializeField] private float EdgeCheckOriginOffset = 0.8f;
    [SerializeField] private float EdgeRayLength = 20f;
    [Tooltip("Minimum Height required for Edge detection and jumping off Edge")]
    [SerializeField] private float minEdgeHeight = 0.75f;

    [Header("Climb Ledge Detection")]
    [SerializeField] private float climbLedgeRayLength = 1.5f;
    [SerializeField] private LayerMask climbLedgeLayer;

    [Header("Ladder Detection")]
    [SerializeField] private float ladderRayLength = 1.5f;
    [SerializeField] private Vector3 ladderRayOriginOffset = new Vector3(0f, 1f, 0f);
    [SerializeField] private LayerMask ladderLayer;
   


    public ObstacleHitData ObstacleCheck()
    {
        var hitData = new ObstacleHitData();
        var forwardRayOrigin = transform.position + forwardRayOffset;

        hitData.forwardHitFound = Physics.Raycast(forwardRayOrigin, transform.forward,
            out hitData.forwardHit, forwardRayLength, obstacleLayer);

        Debug.DrawRay(forwardRayOrigin, transform.forward * forwardRayLength,
            hitData.forwardHitFound ? Color.red : Color.white);

        if (hitData.forwardHitFound)
        {
            // The Height ray will be from "a point above the forwardHitPoint" to the "forwardhitPoint"
            var heightRayOrigin = hitData.forwardHit.point + Vector3.up * heightRayLength;

            hitData.heightHitFound = Physics.Raycast(heightRayOrigin, Vector3.down,
                out hitData.heightHit, heightRayLength, obstacleLayer);

            Debug.DrawRay(heightRayOrigin, Vector3.down* heightRayLength,
            hitData.heightHitFound? Color.red : Color.white);
        }

        return hitData;
    }

    public bool ClimbLedgeCheck(Vector3 dir, out RaycastHit climbLedgeHit)
    {
        climbLedgeHit = new RaycastHit();

        if(dir == Vector3.zero)
            return false;

        // The raycasts must start from above the player's neck
        var origin = transform.position + Vector3.up * 1.5f;
        var offset = new Vector3(0, 0.18f, 0);

        for (int i = 0; i < 10; i++)
        {
            Debug.DrawRay(origin + offset * i, dir);

            if(Physics.Raycast(origin + offset * i, dir, out RaycastHit hit, climbLedgeRayLength, climbLedgeLayer))
            {
                climbLedgeHit = hit;
                return true;
            }
        }

        return false;
    }

    public bool EdgeCheck(Vector3 moveDir, out EdgeHitData edgeHitData)
    {
        edgeHitData = new EdgeHitData();
        if (moveDir == Vector3.zero) return false;
        var origin = transform.position + moveDir * EdgeCheckOriginOffset + Vector3.up;

        if (Utils.ThreeRaycasts(origin, Vector3.down, 0.25f,transform, 
            out List<RaycastHit> hits, EdgeRayLength, obstacleLayer,true))
        {

            var validHits = hits.Where(h => transform.position.y - h.point.y > minEdgeHeight).ToList();

            if(validHits.Count > 0)
            {
                // we need the normal of the ledge so we are casting a ray on the face of the ledge
                var edgeFaceRayOrigin = validHits[0].point;
                // we are moving the ray origin below feet of player (minus a lil value) on the face of ledge
                edgeFaceRayOrigin.y = transform.position.y - 0.1f;

                if(Physics.Raycast(edgeFaceRayOrigin,transform.position-edgeFaceRayOrigin,out RaycastHit edgeFaceHit, 2f, obstacleLayer))
                {
                    Debug.DrawLine(edgeFaceRayOrigin, transform.position, Color.cyan);


                    float obstacleHeight = transform.position.y - validHits[0].point.y;
                    edgeHitData.angle = Vector3.Angle(transform.forward, edgeFaceHit.normal);
                    edgeHitData.height = obstacleHeight;
                    edgeHitData.edgeFaceHit = edgeFaceHit;

                    return true;
                }
             }
        }
        return false;
    }

    public LadderHitData LadderCheck()
    {
        var hitData = new LadderHitData();
        var ladderRayOrigin = transform.position + ladderRayOriginOffset;

        hitData.ladderHitFound= Physics.Raycast(ladderRayOrigin, transform.forward,
            out hitData.ladderHit, ladderRayLength, ladderLayer);

        Debug.DrawRay(ladderRayOrigin, transform.forward * ladderRayLength,
           hitData.ladderHitFound ? Color.red : Color.white);

        //Debug.Log(hitData.ladderHitFound);

        return hitData;
    }

}
public struct ObstacleHitData
{
    public bool forwardHitFound;
    public bool heightHitFound;
    public RaycastHit forwardHit;
    public RaycastHit heightHit;
}

public struct EdgeHitData
{
    public float height;
    public float angle;
    public RaycastHit edgeFaceHit;
}

public struct LadderHitData
{
    public bool ladderHitFound;
    public RaycastHit ladderHit;
}