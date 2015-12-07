using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewGameScript : MonoBehaviour {

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

    public static void LoadLevelSettings () {
        //Update difficulty, and then update parameters based on the new difficulty
        PersistentLevelSettings.settings.difficulty.UpdateDifficultyValue(PersistentLevelSettings.settings.numPlanetsCleared);
        PersistentLevelSettings.settings.difficulty.UpdateTerrainParameters(PersistentTerrainSettings.settings);
        PersistentLevelSettings.settings.difficulty.UpdateLevelParameters(PersistentLevelSettings.settings);
        PersistentLevelSettings.settings.difficulty.UpdateNPOParameters();
        PersistentLevelSettings.settings.difficulty.UpdateObjectiveParameters();
    }

    public void NextLevel()
    {
        PersistentLevelSettings.settings.numPlanetsCleared++;
        PersistentLevelSettings.settings.loadFromSave = false;
        PersistentLevelSettings.settings.savedTime = 0;
        PersistentLevelSettings.settings.enemyBehavior = GroundPathEnemy.Behavior.NORMAL;
        if (PersistentLevelSettings.settings.numPlanetsCleared >= PersistentLevelSettings.settings.numPlanetsTotal)
            Application.LoadLevel(4);
        else {
            PersistentPlayerSettings.settings.health = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter2D>().health;
            LoadLevelSettings(); //Creates a bunch of new objects, loads new terrain
            Application.LoadLevel(3);
        }
    }
}
