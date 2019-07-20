using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sightBox : MonoBehaviour
{
    public GuardBehavior guard;
    public Material Mat;
    public Material SusMat;
    public Material AlertMat;
    public float sightDistance;

    // not on enter, not on exit.
    //this is a huge resource hog? 
    private void OnTriggerStay(Collider other)
    {

        PlayerController player = other.transform.GetComponentInChildren<PlayerController>();
        if (player != null)
        {
            if (Physics.Raycast(guard.interactPoint.position, player.coverCheck.position - guard.interactPoint.position, out RaycastHit hitInfo))
            {
                Debug.DrawRay(guard.interactPoint.position, player.coverCheck.position - guard.interactPoint.position, Color.red);
                if (hitInfo.collider.GetComponent<PlayerController>())
                {
                    guard.determineSight(player);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.transform.root.GetComponentInChildren<PlayerController>();
        if (player != null)
        {
            guard.PlayerInSight = false;
        }
    }
}
