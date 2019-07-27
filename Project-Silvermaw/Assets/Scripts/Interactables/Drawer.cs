using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class Drawer : MonoBehaviour
{
    public float openDist;
    public Vector3 openDir;
    public bool opened = false;

	private void Start()
	{
		GetComponent<Interactable>().OnInteract += Toggle;
	}

	public void Toggle(GameObject subject)
    {
        if (opened)
        {
            transform.Translate(openDir * openDist);
        }
        else
        {
            transform.Translate(-openDir * openDist);
        }

        opened = !opened;
    }
}
