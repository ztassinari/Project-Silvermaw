using System;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class ManaCost : MonoBehaviour
{
	public int Uses = 1;
	public int Cost = 0;
    // Start is called before the first frame update
    void Start()
    {
		GetComponent<Interactable>().OnInteract += Drain;
	}

    public void Drain(GameObject subject)
	{
		PlayerController player = subject.GetComponent<PlayerController>();

		if (player != null && Uses > 0 && player.stats.mana >= Cost)
		{
			player.stats.mana = Math.Max(player.stats.mana-Cost, 0);
			--Uses;
		}
	}
}
