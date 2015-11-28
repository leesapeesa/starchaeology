using UnityEngine;
using System.Collections;

/// <summary>
/// Keeps track of settings pertaining to the user's overall progress between levels of the game.
/// These include how close the player is to meeting the target number of planets to clear, as well
/// as parameters that affect how hard a level is, such as number of enemies.
/// </summary>
public class PersistentLevelSettings : MonoBehaviour {

    public static PersistentLevelSettings settings;

    public const int NUM_PLANETS_EASY = 10;
    public const int NUM_PLANETS_MEDIUM = 15;
    public const int NUM_PLANETS_HARD = 20;

    public int numPlanetsCleared = 0;
    public int numPlanetsTotal = NUM_PLANETS_EASY;
    public int numEnemies = 3;
    public float poisonAmount = 0.1f; //how much the player gets hurt from staying in a poison cloud
    public int numPoisonClouds = 4;
    public int numSlowClouds = 5;
    public int collectCount = 10;
    public int goalCollectCount = 5;
    public int numPlatforms = 5;
    public bool loadFromSave = false; //says whether the currently loading level is being loaded from a savegame
    public int loadSlot = -1; //if the above is true, what slot are we loading from?
    public float savedTime = 0; //how much time had elapsed in the saved level before we saved it?
    public bool enemiesShouldFollowPlayer = false; //should enemies be chasing the player?
    public Difficulty difficulty;

    public Fortunes fortunes;
    public TextAsset fortuneText; //Assign in inspector

    void Awake()
    {
        if (settings == null) {
            DontDestroyOnLoad(gameObject);
            SetDefault();
            settings = this;
        }
        else if (settings != this) {
            Destroy(gameObject);
        }
    }

    public void SetDefault()
    {
        numPlanetsCleared = 0;
        numPlanetsTotal = NUM_PLANETS_EASY;
        poisonAmount = 0.1f;
        numEnemies = 3;
        numPoisonClouds = 4;
        numSlowClouds = 5;
        collectCount = 10;
        goalCollectCount = 5;
        numPlatforms = 5;
        loadFromSave = false;
        loadSlot = -1;
        savedTime = 0;
        difficulty = new Difficulty(DifficultySetting.EASY, NUM_PLANETS_EASY);
        fortunes = new Fortunes (fortuneText);
    }

    /// <summary>
    /// Save any relevant level settings that cannot be reconstructed from the saved difficulty
    /// </summary>
    public void SaveLevelSettings(int slotId)
    {
        PlayerPrefs.SetInt("levelProgress" + slotId, numPlanetsCleared);
        PlayerPrefs.SetInt("totalPlanets" + slotId, numPlanetsTotal);
        PlayerPrefs.SetFloat("savedTime" + slotId, Time.timeSinceLevelLoad);
        PlayerPrefs.SetInt("enemiesFollowPlayer" + slotId, enemiesShouldFollowPlayer ? 1 : 0);
    }

    /// <summary>
    /// Load saved level settings from the specified slot
    /// </summary>
    public void LoadLevelSettings(int slotId)
    {
        loadFromSave = true; //make the level load saved settings rather than randomly generating them
        loadSlot = slotId;
        numPlanetsCleared = PlayerPrefs.GetInt("levelProgress" + slotId);
        numPlanetsTotal = PlayerPrefs.GetInt("totalPlanets" + slotId);
        savedTime = PlayerPrefs.GetFloat("savedTime" + slotId);
        enemiesShouldFollowPlayer = PlayerPrefs.GetInt("enemiesFollowPlayer" + slotId) == 1;
    }
}
