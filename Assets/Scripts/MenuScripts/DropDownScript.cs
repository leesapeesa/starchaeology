using UnityEngine;
using System.Collections;

public class DropDownScript : MonoBehaviour {

    public void SetLevelDifficulty(int value) {
        // Value is the index of the difficulty.
        // Difficulties: [Easy, Medium, Hard]
        print ("Setting Difficulty");
        print (value.ToString ());
        PersistentTerrainSettings.settings.SetDefault ();
        switch (value) {
        case 1:
            PersistentTerrainSettings.settings.SetMediumDifficulty();
            PersistentLevelSettings.settings.numPlanetsTotal = PersistentLevelSettings.NUM_PLANETS_MEDIUM;
            break;
        case 2:
            PersistentTerrainSettings.settings.SetHardDifficulty();
            PersistentLevelSettings.settings.numPlanetsTotal = PersistentLevelSettings.NUM_PLANETS_HARD;
            break;
        default:
            PersistentTerrainSettings.settings.SetDefault();
            PersistentLevelSettings.settings.numPlanetsTotal = PersistentLevelSettings.NUM_PLANETS_EASY;
            break;
        }
    }
}
