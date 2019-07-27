using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class Stealable : MonoBehaviour
{
    public int goldValue;

	private void Start()
	{
		GetComponent<Interactable>().OnInteract += Take;
	}

	public void Take(GameObject subject)
    {
        PlayerController player  = subject.GetComponent<PlayerController>();

        if(player != null)
        {
            player.stats.gold += goldValue;
            Destroy(gameObject);
        }
    }
}
