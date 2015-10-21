using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadGameScript : MonoBehaviour {
    /* Code currently is used to change between levels, eventually will be 
     * added to whatever keeps track of the game progress */
    private bool alreadyLoaded = false;

    // Update is called once per frame
    void Update () {
        bool loadLevel = Input.GetKeyDown(KeyCode.L);

        if (loadLevel) {
            NextLevel();
        }
    }

    public void NewGame() {
        // For the "New game" button in the new game screen
        // should probably be changed to better reflect difficulty
        Application.LoadLevel(1);
    }

    void LoadLevelSettings () {
        PersistentTerrainSettings.settings.LoadLevelSettings();

    }

    public void NextLevel()
    {
        print("terrain difficulties");
        print(PersistentTerrainSettings.settings.difficulty);
        PersistentTerrainSettings.settings.difficulty = (PersistentTerrainSettings.settings.difficulty + 25) % 100;
        PersistentPlayerSettings.settings.health = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter2D>().health;
        LoadLevelSettings(); //Creates a bunch of new objects, loads new terrain
    }

    void Awake () {

    }

}
