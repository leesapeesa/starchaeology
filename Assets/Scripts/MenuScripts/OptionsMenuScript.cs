using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OptionsMenuScript : MonoBehaviour {

    public Button backButton;
    private QualitySettings qualitySettings;

    Resolution[] resolutions;

	// Use this for initialization
	void Start () {
        resolutions = Screen.resolutions;
    }

    public void Back () {
        // This needs to be more complicated in order to actually go to the previous scene,
        // currently just goes to the main menu

        int previousScene = PlayerPrefs.GetInt("previousScene");
        Application.LoadLevel(previousScene); // Load the main menu
    }

}
