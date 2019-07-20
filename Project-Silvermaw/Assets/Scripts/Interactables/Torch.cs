using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : Interactable
{
    public Light lightSource;
    public GameObject flames;
    public int manaCost = 2;

    public override void Interact(GameObject subject)
    {
        PlayerController player = subject.GetComponent<PlayerController>();

        if (player != null)
        {
            if(player.stats.mana >= manaCost && lightSource.enabled == true)
            {
                player.stats.mana -= manaCost;
                lightSource.enabled = false;
                flames.SetActive(false);
            }
        }
    }
}

