using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : Interactable
{
	[SerializeField] private NavMeshObstacle navMeshObstacle = null;
    public GameObject hinge = null;
    public float openThreshold = 90;
    public bool opened = false;


    public override void Interact(GameObject subject)
    {
        if(opened)
        {
            transform.RotateAround(hinge.transform.position, hinge.transform.up, -openThreshold);
		}
        else
        {
            transform.RotateAround(hinge.transform.position, hinge.transform.up, openThreshold);
		}
        opened = !opened;
		navMeshObstacle.enabled = opened;
	}

}
