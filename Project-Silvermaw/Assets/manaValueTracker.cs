using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class manaValueTracker : MonoBehaviour
{
    public PlayerController player;
    Slider slider;

    void Start()
    {
        slider = GetComponentInParent<Slider>();
    }

    void Update()
    {
        slider.value = player.stats.mana;

    }
}
