using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnvironmentScanner : MonoBehaviour
{
    [Header("Obstacle Detection")]
    [SerializeField] Vector3 forwardRayOffset = new Vector3(0f, 0.25f, 0f);
    [SerializeField] float forwardRayLength = 0.8f;
    [SerializeField] float heightRayLength = 5f;
    [SerializeField] LayerMask obstacleLayer;

    [Header("Ledge Detection")]
    [SerializeField] float ledgeRayLength = 10f;
    [Tooltip("Minimum Height required for ledge detection and jumping off ledge")]
    [SerializeField] float minLedgeHeight = 0.75f;

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

    public bool LedgeCheck(Vector3 moveDir)
    {
        if(moveDir == Vector3.zero) return false;

        float originOffset = 0.5f;
        var origin = transform.position + moveDir * originOffset + Vector3.up;

        if(Physics.Raycast(origin, Vector3.down, out RaycastHit hit, ledgeRayLength, obstacleLayer))
        {
            Debug.DrawRay(origin, Vector3.down * ledgeRayLength, Color.green);

           float obstacleHeight = transform.position.y - hit.point.y;
            if(obstacleHeight > minLedgeHeight)
            {
                return true;
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
