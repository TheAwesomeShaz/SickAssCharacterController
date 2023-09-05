using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils 
{
    public static bool ThreeRaycasts(Vector3 origin, Vector3 direction, float spacing, Transform transform,
        out List<RaycastHit> hits, float distance, LayerMask layer,bool debugDraw = false)
    {
        bool centreHitFound = Physics.Raycast(origin,Vector3.down, out RaycastHit centreHit,distance,layer);
        bool leftHitFound = Physics.Raycast(origin - transform.right * spacing,Vector3.down, out RaycastHit leftHit,distance,layer);
        bool rightHitFound = Physics.Raycast(origin + transform.right * spacing,Vector3.down, out RaycastHit rightHit,distance,layer);

        hits = new List<RaycastHit>() { centreHit,leftHit,rightHit};
        
        bool hitFound = centreHitFound || leftHitFound || rightHitFound;

        
        // Gizmos
        //if(hitFound && debugDraw)
        //{
        //    Debug.DrawLine(origin, centreHit.point, Color.red);
        //    Debug.DrawLine(origin - transform.right * spacing, leftHit.point, Color.red);
        //    Debug.DrawLine(origin + transform.right * spacing, rightHit.point, Color.red);
        //}

        return hitFound;

    }

    public static Vector2[] directionArray = new Vector2[]
    {
        new Vector2(0,1),  // Up 
        new Vector2(1,0),  // Right
        new Vector2(-1,0), // Left
        new Vector2(0,1), // Down

        new Vector2(1,1), // TopRight
        new Vector2(-1,1), // TopLeft
        new Vector2(1,-1), // BottomRight
        new Vector2(-1,-1), // BottomLeft
};


}
