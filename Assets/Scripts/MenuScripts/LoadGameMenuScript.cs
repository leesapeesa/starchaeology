using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadGameMenuScript : MonoBehaviour {

    public Button startButton;

    // Use this for initialization
    void Start() {
        startButton = startButton.GetComponent<Button>();
    }

    public void Back() {
        int previousScene = PlayerPrefs.GetInt("previousScene");
        Application.LoadLevel(previousScene); // Load the main menu
    }
}
