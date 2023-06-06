using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnvironmentScanner : MonoBehaviour
{
    [Header("Obstacle Detection")]
    [SerializeField] Vector3 forwardRayOffset = new Vector3(0f, 0.25f, 0f);
    [SerializeField] float forwardRayLength = 1f;
    [SerializeField] float heightRayLength = 5f;
    [SerializeField] LayerMask obstacleLayer;

    [Header("Ledge Detection")]
    [SerializeField] float ledgeCheckOriginOffset = 0.8f;
    [SerializeField] float ledgeRayLength = 10f;
    [Tooltip("Minimum Height required for ledge detection and jumping off ledge")]
    [SerializeField] float minLedgeHeight = 0.75f;

    [Header("Climb Ledge Detection")]
    [SerializeField] float climbLedgeRayLength = 1.5f;
    [SerializeField] LayerMask climbLedgeLayer;

    public ObstacleHitData ObstacleCheck()
    {
        var hitData = new ObstacleHitData();
        var forwardRayOrigin = transform.position + forwardRayOffset;

        hitData.forwardHitFound = Physics.Raycast(forwardRayOrigin, transform.forward,
            out hitData.forwardHit, forwardRayLength, obstacleLayer);

        Debug.DrawRay(forwardRayOrigin, transform.forward * forwardRayLength,
            hitData.forwardHitFound?Color.red:Color.white);

        if(hitData.forwardHitFound)
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

    public bool EdgeLedgeCheck(Vector3 moveDir, out LedgeHitData edgeLedgeHitData)
    {
        edgeLedgeHitData = new LedgeHitData();

        if(moveDir == Vector3.zero) return false;

        var origin = transform.position + moveDir * ledgeCheckOriginOffset + Vector3.up;

        if(PhysicsUtil.ThreeRaycasts(origin, Vector3.down, 0.25f,transform, 
            out List<RaycastHit> hits, ledgeRayLength, obstacleLayer,true))
        {
            var validHits = hits.Where(h => transform.position.y - h.point.y > minLedgeHeight).ToList();

            if(validHits.Count > 0)
            {
                // we need the normal of the ledge so we are casting a ray on the face of the ledge
                var ledgeFaceRayOrigin = validHits[0].point;
                // we are moving the ray origin below feet of player (minus a lil value) on the face of ledge
                ledgeFaceRayOrigin.y = transform.position.y - 0.1f;

                if(Physics.Raycast(ledgeFaceRayOrigin,transform.position-ledgeFaceRayOrigin,out RaycastHit ledgeFaceHit, 2f, obstacleLayer))
                {
                    Debug.DrawLine(ledgeFaceRayOrigin, transform.position, Color.cyan);


                    float obstacleHeight = transform.position.y - validHits[0].point.y;
                    edgeLedgeHitData.angle = Vector3.Angle(transform.forward, ledgeFaceHit.normal);
                    edgeLedgeHitData.height = obstacleHeight;
                    edgeLedgeHitData.ledgeFaceHit = ledgeFaceHit;

                        return true;
                }
        }
        }
        return false;
    }


}
public struct ObstacleHitData
{
    public bool forwardHitFound;
    public bool heightHitFound;
    public RaycastHit forwardHit;
    public RaycastHit heightHit;
}

public struct LedgeHitData
{
    public float height;
    public float angle;
    public RaycastHit ledgeFaceHit;
}