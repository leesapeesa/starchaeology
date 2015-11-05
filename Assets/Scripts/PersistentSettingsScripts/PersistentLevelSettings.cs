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
    public Difficulty difficulty;

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
        difficulty = new Difficulty(DifficultySetting.EASY, NUM_PLANETS_EASY);
    }
}
