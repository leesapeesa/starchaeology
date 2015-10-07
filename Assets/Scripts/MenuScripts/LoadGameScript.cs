using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadGameScript : MonoBehaviour {
    /* Code currently is used to change between levels, eventually will be 
     * added to whatever keeps track of the game progress */
    private PersistentSettings settings;
    private bool alreadyLoaded = false;

    // Update is called once per frame
    void Update () {
        bool loadLevel = Input.GetKeyDown(KeyCode.L);

        if (loadLevel) {
			print ("terrain difficulties");
			print (settings.ptSettings.difficulty);
            settings.ptSettings.difficulty = (settings.ptSettings.difficulty + 25) % 100;
            LoadLevelSettings(); //Creates a bunch of new objects, loads new terrain
        }
    }

    public void NewGame() {
        // For the "New game" button in the new game screen
        // should probably be changed to better reflect difficulty
        Application.LoadLevel(1);
    }

    void LoadLevelSettings () {
        settings.ptSettings.LoadLevelSettings();

    }

    void Awake () {
        //DontDestroyOnLoad(transform.gameObject);
        GameObject settingsObject = GameObject.FindWithTag("All Settings");
        settings = settingsObject.GetComponent<PersistentSettings>();
        if (!alreadyLoaded) {
            alreadyLoaded = true;
        }
    }

}
