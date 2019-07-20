using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldTracker : MonoBehaviour
{
    public GameController game;
    public PlayerController player;
    public Text goldText;


    void Update()
    {
        goldText.text = player.stats.gold.ToString() + "/" + game.winScore.ToString();

        if(player.stats.gold >= game.winScore)
        {
            game.Win();
        }
    }
}
