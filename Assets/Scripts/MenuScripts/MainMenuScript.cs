using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuScript : MonoBehaviour {

    public Canvas saveGameMenu;
    public Canvas loadGameMenu;
    public Canvas optionsMenu;
    public Canvas helpMenu;

    void Start () {
        saveGameMenu.enabled = false;
        loadGameMenu.enabled = false;
        optionsMenu.enabled = false;
        helpMenu.enabled = false;
    }

    public void Back () {
        saveGameMenu.enabled = false;
        loadGameMenu.enabled = false;
        optionsMenu.enabled = false;
        helpMenu.enabled = false;
    }

    public void NewGame () {
        Application.LoadLevel(5);
    }

    public void QuitGame () {
        Application.Quit();
    }

    public void Options () {
        optionsMenu.enabled = true;
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
}
