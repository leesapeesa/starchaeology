using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class NewGameMenuScript : MonoBehaviour {

    public Button startButton;
    public InputField playerNameInput;

    // Use this for initialization
    void Start () {
        Time.timeScale = 1;
        //Set all parameters to their default values for the start of the game.
        PersistentTerrainSettings.settings.SetDefault();
        if (PersistentPlayerSettings.settings != null)
            PersistentPlayerSettings.settings.SetDefault();
        PersistentLevelSettings.settings.SetDefault();
    }

    // Should be changed to reflect how we end up loading the game
    public void NewGame() {
        NewGameScript.LoadLevelSettings();
        PersistentPlayerSettings.settings.playerName = playerNameInput.text;
        SetVolume.instance.changeMusic();
        Application.LoadLevel(3);
    }


}
