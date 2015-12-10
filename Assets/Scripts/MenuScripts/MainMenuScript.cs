using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuScript : MonoBehaviour {

    public Canvas saveGameMenu;
    public Canvas loadGameMenu;
    public Canvas helpMenu;
    public Text highScoreText;

    void Start () {

        saveGameMenu.enabled = false;
        loadGameMenu.enabled = false;
        helpMenu.enabled = false;

        DisplayHighScore();
    }

    public void Back () {
        saveGameMenu.enabled = false;
        loadGameMenu.enabled = false;
        helpMenu.enabled = false;
    }

    public void NewGame () {
        Application.LoadLevel(5);
    }

    public void QuitGame () {
        Application.Quit();
    }

    public void Options () {
        Application.LoadLevel(7);
    }

    public void Help () {
        helpMenu.enabled = true;
    }

    public void LoadGame () {
        loadGameMenu.enabled = true;
    }
    
    public void Credits () {
        Application.LoadLevel(6);
    }

    private void DisplayHighScore() {
        if (PlayerPrefs.HasKey("HighScore")) {
            highScoreText.text = "High Score: " + PlayerPrefs.GetInt("HighScore").ToString();
        } else {
            PlayerPrefs.SetInt("HighScore", 0);
            highScoreText.text = "High score: 0";
        }
    }
}
