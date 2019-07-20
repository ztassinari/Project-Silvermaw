using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCam : MonoBehaviour
{

    // https://www.youtube.com/watch?v=blO039OzUZc

    Vector2 mouseLook;
    Vector2 smoothV;
    public float sensitivity = 5.0f;
    public float smoothing = 2.0f;
    public float lookHeightMax = 80;
    public float lookHeightMin = -70;

    public Transform target;
    public PlayerController playerController;
    public GameController gameController;


    void Start()
    {
        SetCameraTarget(target);
    }

    void LateUpdate()
    {
        if (!gameController.paused)
        {
            LookAtTarget();
        }
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

    void LookAtTarget()
    {
        var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing);
        mouseLook += smoothV;
        mouseLook.y = Mathf.Clamp(mouseLook.y, lookHeightMin, lookHeightMax);
        //Debug.Log(mouseLook.y);

        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        target.localRotation = Quaternion.AngleAxis(mouseLook.x, target.transform.up);
    }
}
