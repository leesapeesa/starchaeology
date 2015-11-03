using UnityEngine;
using System.Collections;

public class PersistentLevelSettings : MonoBehaviour {

    public static PersistentLevelSettings settings;

    public const int NUM_PLANETS_EASY = 10;
    public const int NUM_PLANETS_MEDIUM = 15;
    public const int NUM_PLANETS_HARD = 20;

    public int numPlanetsCleared = 0;
    public int numPlanetsTotal = NUM_PLANETS_EASY;

    void Awake()
    {
        if (settings == null) {
            DontDestroyOnLoad(gameObject);
            settings = this;
        }
        else if (settings != this) {
            Destroy(gameObject);
        }
    }

    public void SetDefault()
    {
        numPlanetsCleared = 0;
    }
}
