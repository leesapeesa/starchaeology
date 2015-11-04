using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Identifies which overall difficulty setting the player chose for this game.
/// </summary>
public enum DifficultySetting
{
    EASY,
    MEDIUM,
    HARD
}

/// <summary>
/// Represents the current difficulty of the game, and contains methods to update parameters based on current difficuly.
/// While difficulty is not itself a PersistentObject, a copy of Difficulty should be carried around by one of the
/// PersistentObjects.
/// </summary>
public class Difficulty {

    private int difficulty; ///Underlying numeric encoding of the difficulty. Higher values = harder
    private DifficultySetting setting;
    private int numLevels;
    private readonly Dictionary<DifficultySetting, int> startValues;
    private readonly Dictionary<DifficultySetting, int> endValues;
    //Constant coefficients relating the internal numeric difficulty encoding to various parameters
    private const float HEIGHT_COEFF = 1f / 4f;
    private const float HEIGHT_OFFSET = 5;
    private const float ENEMIES_COEFF = 1f / 12f;
    private const float ENEMIES_OFFSET = 2;
    private const float SIDE_LENGTH_COEFF = 5;
    private const float POISON_COEFF = 1f / 400f;
    private const float FREQ_COEFF = 1f / 10f;
    private const float FREQ_OFFSET = 4;
    private const float OCTAVES_COEFF = 1f / 50f;
    private const float OCTAVES_OFFSET = 1;
    private const float LACUNARITY_COEFF = 1f / 125f;
    private const float LACUNARITY_OFFSET = 1;
    private const float GAIN_COEFF = 1f / 375f;
    private const float GROUND_PATH_ENEMY_DAMAGE_COEFF = 1f / 7f;

    public Difficulty(DifficultySetting startSetting, int levels)
    {
        //Initialize the lookup tables for starting and ending difficulties
        startValues = new Dictionary<DifficultySetting, int>()
        {
            { DifficultySetting.EASY, 20 },
            { DifficultySetting.MEDIUM, 40 },
            { DifficultySetting.HARD, 75 }
        };
        endValues = new Dictionary<DifficultySetting, int>()
        {
            { DifficultySetting.EASY, 120 },
            { DifficultySetting.MEDIUM, 200 },
            { DifficultySetting.HARD, 375 }
        };

        numLevels = levels;
        setting = startSetting;
        difficulty = startValues[setting];
    }

    /// <summary>
    /// Increase difficulty based on player progress
    /// </summary>
    public void UpdateDifficultyValue(int curLevel)
    {
        //difficulty increases linearly with increasing level. The slope is determined by the start and end difficulty
        //values and the number of levels.
        difficulty = (int)(startValues[setting] + curLevel * (float)(endValues[setting] - startValues[setting]) / numLevels);
    }

    /// <summary>
    /// Update the values of the PersistentTerrainSettings parameters based on current level
    /// </summary>
    public void UpdateTerrainParameters(PersistentTerrainSettings settings)
    {
        settings.height = HEIGHT_OFFSET + HEIGHT_COEFF * difficulty;
        settings.sideLength = SIDE_LENGTH_COEFF * difficulty;
        settings.frequency = FREQ_OFFSET + FREQ_COEFF * difficulty;
        settings.octaves = (int)(OCTAVES_OFFSET + OCTAVES_COEFF * difficulty);
        settings.lacunarity = LACUNARITY_OFFSET + LACUNARITY_COEFF * difficulty;
        settings.gain = GAIN_COEFF * difficulty;

        if (difficulty > 30 && difficulty < 70) {
            settings.textureType = TerrainTextureType.Grassy;
        } else if (difficulty >= 0 && difficulty < 30) {
            settings.textureType = TerrainTextureType.Desert;
        } else {
            settings.textureType = TerrainTextureType.Rocky;
        }
    }

    /// <summary>
    /// Update the values of the PersistentLevelSettings parameters based on current level
    /// </summary>
    public void UpdateLevelParameters(PersistentLevelSettings settings)
    {
        settings.numEnemies = (int)(ENEMIES_OFFSET + ENEMIES_COEFF * difficulty);
        settings.poisonAmount = POISON_COEFF * difficulty;
    }

    /// <summary>
    /// Any enemy type that has level-specific static parameters should get those parameters
    /// updated here.
    /// </summary>
    public void UpdateEnemyParameters()
    {
        GroundPathEnemy.contactDamage = GROUND_PATH_ENEMY_DAMAGE_COEFF * difficulty;
    }
	
}
