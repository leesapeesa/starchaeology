using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class InGameMenuScript : MonoBehaviour {

    public Canvas pauseMenu;

    public Canvas saveGameMenu;
    public Canvas loadGameMenu;
    public Canvas optionsMenu;
    public Canvas helpMenu;
    public Canvas lossScreen;
    public Canvas winScreen;

    public Button pauseButton;

    private bool isPaused;

	// Use this for initialization
	void Start () {
        // Hide menus that shouldn't be in view
        pauseMenu.enabled = false;
        DisableMenus();
	}

    public void Update() {

        if (Input.GetKeyDown("escape") && !isPaused) {
            PausePress();
        } else if (Input.GetKeyDown("escape") && isPaused) {
            Resume();
        }
    }

    public void PausePress () {
        isPaused = true;
        pauseMenu.enabled = true;
        pauseButton.enabled = false;
        Time.timeScale = 0;
    }

    public void Back () {
        isPaused = false;
        pauseButton.enabled = true;
        DisableMenus();
        Time.timeScale = 1;
    }

    public void Resume () {
        isPaused = false;
        pauseMenu.enabled = false;
        pauseButton.enabled = true;
        DisableMenus();
        Time.timeScale = 1;

    }

    private void DisableMenus () {
        saveGameMenu.enabled = false;
        loadGameMenu.enabled = false;
        optionsMenu.enabled = false;
        helpMenu.enabled = false;
        lossScreen.enabled = false;
        winScreen.enabled = false;
    }
    

    // Pause menu options
    public void QuitGame () {
        Time.timeScale = 1;
        Application.Quit();
    }

    public void MainMenu () {
        Time.timeScale = 1;
        Application.LoadLevel(0);
    }

    public void Options() {
        optionsMenu.enabled = true;
    }

    public void Help() {
        helpMenu.enabled = true;
    }

    public void LoadGame() {
        loadGameMenu.enabled = true;
    }

    public void SaveGame() {
        saveGameMenu.enabled = true;
    }
}
