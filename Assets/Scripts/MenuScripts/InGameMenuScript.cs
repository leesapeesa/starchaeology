using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGameMenuScript : MonoBehaviour {

    public Canvas pauseMenu;
    public Button pauseButton;

	// Use this for initialization
	void Start () {
        pauseMenu = pauseMenu.GetComponent<Canvas>();
        pauseButton = pauseButton.GetComponent<Button>();
        pauseMenu.enabled = false;
	}
	
    public void PausePress () {
        pauseMenu.enabled = true;
        pauseButton.enabled = false;
        Time.timeScale = 0;

        // enable buttons in the pause menu here
    }

    public void ResumePress () {
        pauseMenu.enabled = false;
        pauseButton.enabled = true;
        Time.timeScale = 1;
    }

    public void ExitGame () {
        Application.Quit();
    }
}
