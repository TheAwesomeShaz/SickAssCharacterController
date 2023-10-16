using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Watch the vid Explaination lmao
// https://www.youtube.com/watch?v=NeuxiCn_zR8&list=PLD_vBJjpCwJsqpD8QRPNPMfVUpPFLVGg4&index=5&ab_channel=SebastianGraves

public class CameraManager : MonoBehaviour
{
    [field: SerializeField] private InputManager inputManager { get; set; }

    public Transform targetTransform; // The object the camera will follow
    public Transform cameraPivotTransform; // The object the camera uses to pivot (move up and down)
    public Transform cameraTransform;
    public LayerMask collisionLayers; // Layers we want our camera to collide with
    private float defaultPosition;
    private Vector3 cameraFollowVelocity = Vector3.zero;
    private Vector3 cameraVectorPosition;

    public float cameraCollisionOffset = 0.2f; //How much camera will jump off of objects it is colliding with
    public float cameraCollisionRadius = 0.2f;
    public float cameraFollowSpeed = 0.2f;
    public float cameraLookSpeed = 2f; // speed while looking up down
    public float cameraPivotSpeed = 2f; // speed while looking left right
    public float minimumCollisionOffset = 0.1f; // How much distance camera maintains from player while jumping off of objects it is colliding with
    public float cameraCollisionAdjustSpeed = 0.2f; // Speed with which camera will jump off when colliding 

    public float lookAngle; // for looking Right and Left
    public float pivotAngle; // for looking Up and Down

    [SerializeField] float minPivotAngle = -40f;
    [SerializeField] float maxPivotAngle = 70f;

    private void Awake()
    {
        targetTransform = FindObjectOfType<PlayerStateMachine>().transform;
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        HandleCameraCollisions();
    }

    void FollowTarget()
    {
        // ref keyword here means passing by reference, the cameraFollowVelocity will change depending on the smoothDamp. These Changes will not only be done in Vector3.SmoothDamp() but also in our original variable This is how Vector3.SmoothDamp() changes the velocity of cam movement and smoothdamps the motion from the camera position to the target position

        Vector3 targetPositon = Vector3.SmoothDamp(transform.position, targetTransform.position,
            ref cameraFollowVelocity, cameraFollowSpeed);

        transform.position = targetPositon;
    }

    void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;

        lookAngle += (inputManager.CameraInputX * cameraLookSpeed);
        pivotAngle += (-inputManager.CameraInputY * cameraPivotSpeed);
        //Limit the rotation so that cam cant rotate 360 in vertical direction
        pivotAngle = Mathf.Clamp(pivotAngle, minPivotAngle, maxPivotAngle);

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivotTransform.localRotation = targetRotation;
    }

    void HandleCameraCollisions()
    {
        float targetPosition = defaultPosition;
        // directing the raycast from the pivot(basically the player position) to the camera
        Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction,
            out RaycastHit hit, Mathf.Abs(targetPosition), collisionLayers))
        {
            float distance = Vector3.Distance(cameraPivotTransform.position, hit.point);   
            targetPosition =-(distance-cameraCollisionOffset);
        }
        if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition -= minimumCollisionOffset;
        }
        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, Time.deltaTime/cameraCollisionAdjustSpeed);
        cameraTransform.localPosition = cameraVectorPosition;   
    }

    // TODO: whenever UI is made maybe change sensitivity like this?
    //void SetSensitivity(float xSensitivty, float ySensitivity)
    //{
    //    cameraLookSpeed = xSensitivty;
    //    cameraPivotSpeed = ySensitivity;
    //}
}
