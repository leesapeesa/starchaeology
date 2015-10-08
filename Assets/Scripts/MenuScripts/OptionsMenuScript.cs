using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OptionsMenuScript : MonoBehaviour {

    public Button backButton;
    public Dropdown resolutionsDropdown;
    public Dropdown qualityDropdown;

    Resolution[] resolutions;

    // Use this for initialization
    void Start () {
        resolutionsDropdown = resolutionsDropdown.GetComponent<Dropdown>();
        qualityDropdown = qualityDropdown.GetComponent<Dropdown>();

        resolutions = Screen.resolutions;
        resolutionsDropdown.options.Clear();

        for (int i = 0; i < resolutions.Length; ++i) {
            Resolution res = resolutions[i];
            resolutionsDropdown.options.Add(new Dropdown.OptionData(ResToString(res)));
            
        }

        string[] qualities = QualitySettings.names;
        qualityDropdown.options.Clear();

        for (int i = 0; i < qualities.Length; ++i) {
            qualityDropdown.options.Add(new Dropdown.OptionData(qualities[i]));
        }
    }

    string ResToString (Resolution res) {
        // Helper method that translates a screen resolution to a more
        // readable format
        return res.width + "x" + res.height;
    }

    public void UpdateResolution () {
        Screen.SetResolution(resolutions[resolutionsDropdown.value].width, resolutions[resolutionsDropdown.value].height, Screen.fullScreen);
    }
    
    public void UpdateQuality () {
        QualitySettings.SetQualityLevel(qualityDropdown.value);
    }

    public void Back () {
        Application.LoadLevel(0); // Load the main menu
    }

}
