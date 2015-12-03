using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class InGameMenuScript : MonoBehaviour {

    public Canvas inventory;

    public Canvas pauseMenu;
    public Canvas saveGameMenu;
    public Canvas loadGameMenu;
    public Canvas optionsMenu;
    public Canvas helpMenu;
    public Canvas lossScreen;
    public Canvas winScreen;

    public Button inventoryButton;
    public Button pauseButton;
    public Text timerText;
    public Text objectivesText;


    private bool isPaused;

    // Use this for initialization
    void Start () {
        // Hide menus that shouldn't be in view
        pauseMenu.enabled = false;
        DisableMenus();
    }

    public void Update() {

        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused) {
            print("Not paused, escape press");
            PausePress();
        } else if (Input.GetKeyDown(KeyCode.Escape) && isPaused) {
            print("Paused, escape press");
            Resume();
        }
    }

    public void PausePress () {
        if (isPaused == false) {
            isPaused = true;
            pauseMenu.enabled = true;
            Time.timeScale = 0;
        } else {
            isPaused = false;
            pauseMenu.enabled = false;
            Time.timeScale = 1;
        }
    }

    public void Back () {
        isPaused = false;
        pauseButton.interactable = true;
        DisableMenus();
    }

    public void Resume () {
        isPaused = false;
        pauseMenu.enabled = false;
        pauseButton.interactable = true;
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
