using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class InGameMenuScript : MonoBehaviour {

    public Canvas pauseMenu;
    public Button pauseButton;

    private bool isPaused_;

	// Use this for initialization
	void Start () {
        pauseMenu = pauseMenu.GetComponent<Canvas>();
        pauseButton = pauseButton.GetComponent<Button>();
        pauseMenu.enabled = false;
	}
	
    public void PausePress () {
        isPaused_ = true;
        pauseMenu.enabled = true;
        pauseButton.enabled = false;
        Time.timeScale = 0;

        // enable buttons in the pause menu here
    }

    public void ResumePress () {
        isPaused_ = false;
        pauseMenu.enabled = false;
        pauseButton.enabled = true;
        Time.timeScale = 1;
    }

    public void QuitGame () {
        Application.Quit();
    }

    public void MainMenu () {
        Application.LoadLevel(0);
    }

    public void Update () {

        if (Input.GetKeyDown("escape") && !isPaused_) {
            PausePress();   
        } else if (Input.GetKeyDown("escape") && isPaused_) {
            ResumePress();
        }
    }
}
