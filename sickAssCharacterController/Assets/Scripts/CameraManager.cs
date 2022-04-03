using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform targetTransform; // The object the camera will follow
    Vector3 cameraFollowVelocity = Vector3.zero;

    public float cameraFollowSpeed = 0.2f;

    private void Awake()
    {
        targetTransform = FindObjectOfType<PlayerManager>().transform;
    }

    public void FollowTarget()
    {
        // ref keyword here means passing by reference, the cameraFollowVelocity will change depending on the smoothDamp. These Changes will not only be done in Vector3.SmoothDamp() but also in our original variable This is how Vector3.SmoothDamp() changes the velocity of cam movement and smoothdamps the motion from the camera position to the target position

        Vector3 targetPositon = Vector3.SmoothDamp(transform.position, targetTransform.position,
            ref cameraFollowVelocity, cameraFollowSpeed);

        transform.position = targetPositon;
    }

    public RotateCamera()
    {

    }
}
