using UnityEngine;
using System.Collections;
using System;

public class LoadGameScript : MonoBehaviour {
    /* Code currently is used to change between levels, eventually will be 
     * added to whatever keeps track of the game progress */
    private PersistentTerrainSettings settings;
    private int difficulty = 0;
    private bool alreadyLoaded = false;

	// Update is called once per frame
	void Update () {
        bool loadLevel = Input.GetKeyDown(KeyCode.L);

        if (loadLevel) {
            difficulty = (difficulty + 25) % 100;
            Console.WriteLine("difficulty is %d", difficulty);
            LoadLevelSettings();
        }
    }

    void LoadLevelSettings () {
            settings.LoadLevelSettings(difficulty);
    }

    void Awake () {
        DontDestroyOnLoad(transform.gameObject);
        GameObject settingsObject = GameObject.FindWithTag("Settings");
        settings = settingsObject.GetComponent<PersistentTerrainSettings>();
        if (!alreadyLoaded) {
            alreadyLoaded = true;
        }
    }

}
