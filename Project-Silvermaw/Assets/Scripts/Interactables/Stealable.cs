using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stealable : Interactable
{
    public int goldValue;

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public override void Interact(GameObject subject)
    {
        PlayerController player  = subject.GetComponent<PlayerController>();

        if(player != null)
        {
            player.stats.gold += goldValue;
            Destroy(gameObject);
        }
    }
}
