using UnityEngine;
using System.Collections;

public class DropDownScript : MonoBehaviour {

    public void SetLevelDifficulty(int value) {
        // Value is the index of the difficulty.
        // Difficulties: [Easy, Medium, Hard]
        PersistentTerrainSettings.settings.SetDefault ();
        switch (value) {
        case 1:
            PersistentLevelSettings.settings.numPlanetsTotal = PersistentLevelSettings.NUM_PLANETS_MEDIUM;
            PersistentLevelSettings.settings.difficulty = new Difficulty(DifficultySetting.MEDIUM, 
                                                                         PersistentLevelSettings.NUM_PLANETS_MEDIUM);
            break;
        case 2:
            PersistentLevelSettings.settings.numPlanetsTotal = PersistentLevelSettings.NUM_PLANETS_HARD;
            PersistentLevelSettings.settings.difficulty = new Difficulty(DifficultySetting.HARD,
                                                                         PersistentLevelSettings.NUM_PLANETS_HARD);
            break;
        default:
            PersistentLevelSettings.settings.numPlanetsTotal = PersistentLevelSettings.NUM_PLANETS_EASY;
            PersistentLevelSettings.settings.difficulty = new Difficulty(DifficultySetting.EASY,
                                                                         PersistentLevelSettings.NUM_PLANETS_EASY);
            break;
        }
    }
}
