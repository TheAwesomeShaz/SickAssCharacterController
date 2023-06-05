using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsUtil 
{
    public static bool ThreeRaycasts(Vector3 origin, Vector3 direction, float spacing, Transform transform,
        out List<RaycastHit> hits, float distance, LayerMask layer,bool debugDraw = false)
    {
        bool centreHitFound = Physics.Raycast(origin,Vector3.down, out RaycastHit centreHit,distance,layer);
        bool leftHitFound = Physics.Raycast(origin - transform.right * spacing,Vector3.down, out RaycastHit leftHit,distance,layer);
        bool rightHitFound = Physics.Raycast(origin + transform.right * spacing,Vector3.down, out RaycastHit rightHit,distance,layer);

        hits = new List<RaycastHit>() { centreHit,leftHit,rightHit};
        
        bool hitFound = centreHitFound || leftHitFound || rightHitFound;


        if(hitFound && debugDraw)
        {
            Debug.DrawLine(origin, centreHit.point, Color.red);
            Debug.DrawLine(origin - transform.right * spacing, leftHit.point, Color.red);
            Debug.DrawLine(origin + transform.right * spacing, rightHit.point, Color.red);
        }

        return hitFound;

    }
       
}
