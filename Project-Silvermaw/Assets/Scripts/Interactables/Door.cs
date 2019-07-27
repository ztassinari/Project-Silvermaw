using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Interactable))]
public class Door : MonoBehaviour
{
	[SerializeField] private NavMeshObstacle navMeshObstacle = null;
    public GameObject hinge = null;
    public float openThreshold = 90;
    public bool opened = false;

	private void Start()
	{
		GetComponent<Interactable>().OnInteract += Toggle;
	}

	public void Toggle(GameObject subject)
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
