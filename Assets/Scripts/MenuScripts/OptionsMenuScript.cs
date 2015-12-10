using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OptionsMenuScript : MonoBehaviour {

    public Button backButton;
    public Dropdown resolutionsDropdown;
    public Dropdown qualityDropdown;
    public Slider soundSlider;
    public Slider musicSlider;
    private AudioSource musicSource;

    Resolution[] resolutions;

    // Use this for initialization
    void Start () {
        musicSource = GameObject.FindGameObjectWithTag("Volume").GetComponent<AudioSource>();

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

        if (PlayerPrefs.HasKey("Volume")) {
            AudioListener.volume = PlayerPrefs.GetFloat("Volume");
            soundSlider.value = AudioListener.volume;
        }
        if (PlayerPrefs.HasKey("MusicVolume")) {
            musicSource.volume = PlayerPrefs.GetFloat("MusicVolume");
            musicSlider.value = AudioListener.volume;
        }

    }

    void Update () {
        PlayerPrefs.SetFloat("Volume", soundSlider.value);
        AudioListener.volume = soundSlider.value;
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        musicSource.volume = musicSlider.value;

    }

    public void Back () {
        Application.LoadLevel(0);
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

}
