using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sneakValueTracker : MonoBehaviour
{
    public PlayerController player;
    Slider slider;
    public Image fillArea;
    public Gradient fillColor;

    void Start()
    {
        slider = GetComponentInParent<Slider>();
    }
    
    void Update()
    {
        slider.value = player.luminance * 100;
        fillArea.color = fillColor.Evaluate(player.luminance);

    }
}
