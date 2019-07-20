using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawer : Interactable
{
    public float openDist;
    public Vector3 openDir;
    public bool opened = false;

    public override void Interact(GameObject subject)
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
