using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float lookSmooth = 0.09f;//how fast the camera turns to target
    public Vector3 offsetFromTarget = new Vector3(0, 6, -8);
    public float xTilt = 10; // how far camera is rotated on x axis

    Vector3 destination = Vector3.zero;
    PlayerController playerController;
    float rotateVel = 0;

    // Start is called before the first frame update
    void Start()
    {
        SetCameraTarget(target);
    }

    public void SetCameraTarget(Transform t)
    {
        target = t;

        if (target != null)
        {
            if (target.GetComponent<PlayerController>())
            {
                playerController = target.GetComponent<PlayerController>();
            }
            else
            {
                Debug.LogError("Camera needs a player controller!");
            }
        }
        else
        {
            Debug.LogError("Camera has no target!");
        }
    }

    // very last update in frame, for moving and rotating. Makes sure that the camera accurately reflects where char is. 
    //(other calc during frame)
    void LateUpdate()
    {
        MoveToTarget();
        LookAtTarget();
    }

    void MoveToTarget()
    {
        //rotates destination-- multiplies offset by the rotation of the playerController--rotates it
        //destination = playerController.TargetRotation() * offsetFromTarget;
        // makes destination relative to target
        destination += target.position;
        transform.position = destination;
    }
    //rotates only on y
    void LookAtTarget()
    {
        //SmoothDampAngle is "similar to a lerp, but smoother". 
        float eulerYAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, target.eulerAngles.y, ref rotateVel, lookSmooth);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, eulerYAngle, 0);
    }
}
