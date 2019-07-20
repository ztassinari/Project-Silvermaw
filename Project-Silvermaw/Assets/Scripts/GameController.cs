using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public bool paused = false;
    public GameObject pausePanel;
    public GameObject endPanel;
    public Text endText; 

    public PlayerController player;
    public int winScore;
    

    void Start()
    {
        Time.timeScale = 1;
        endPanel.SetActive(false);
        paused = false;
    }
    public void Win()
    {
        paused = true; 

        Time.timeScale = 0;
        endPanel.SetActive(true);
        endText.text = "You Win!";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Lose()
    {
        paused = true;

        Time.timeScale = 0;
        endPanel.SetActive(true);
        endText.text = "You Lose!";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void TogglePause()
    {
        paused = !paused;

        if (paused == true)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
        }
        else
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }
}
