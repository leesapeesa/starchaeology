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
    private bool levelEnded;
    private const float KEY_PRESS_DELAY = 0.5f; //how long to wait before accepting level end keypresses

    // Use this for initialization
    void Start () {
        // Hide menus that shouldn't be in view
        pauseMenu.enabled = false;
        DisableMenus();

        levelEnded = false;
    }

    public void Update() {

        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused) {
            PausePress();
        } else if (Input.GetKeyDown(KeyCode.Escape) && isPaused) {
            Resume();
        }

        //If the player has won or lost, start listening for any key press
        if ((winScreen.enabled || lossScreen.enabled) && !levelEnded) {
            levelEnded = true;
            StartCoroutine(DelayedKeyPressListener(KEY_PRESS_DELAY));
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
        LevelUtil.EndGame(LevelUtil.GameEndType.MAIN_MENU);
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

    /// <summary>
    /// Listen for key press to go to next level. Includes a small delay before accepting key presses,
    /// so that stray movements are not immediately caught.
    /// </summary>
    private IEnumerator DelayedKeyPressListener(float delay)
    {
        //can't use WaitForSeconds here because timeScale is 0 at end of level.
        //Instead we have to manually handle time delay
        while (delay > 0) {
            delay -= Time.unscaledDeltaTime;
            yield return null;
        }
        while (true) {
            // Handling key inputs for the win and loss screens
            if (Input.anyKey && lossScreen.enabled) {
                Time.timeScale = 1;
                LevelUtil.EndGame(LevelUtil.GameEndType.NEW_GAME);
            }
            else if (Input.anyKey && winScreen.enabled) {
                Time.timeScale = 1;
                GameObject.FindWithTag("LoadGame").GetComponent<NewGameScript>().NextLevel();
            }
            yield return null;
        }
    }
}
