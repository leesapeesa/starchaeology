using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class NewGameMenuScript : MonoBehaviour {

    public Button startButton;

    // Use this for initialization
    void Start () {
        //Set all parameters to their default values for the start of the game.
        PersistentTerrainSettings.settings.SetDefault();
        if (PersistentPlayerSettings.settings != null)
            PersistentPlayerSettings.settings.SetDefault();
        PersistentLevelSettings.settings.SetDefault();
    }

    // Should be changed to reflect how we end up loading the game
    public void NewGame() {
        LoadGameScript.LoadLevelSettings();
        Application.LoadLevel(3);
    }


}
