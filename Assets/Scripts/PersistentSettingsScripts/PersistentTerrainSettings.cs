using UnityEngine;
using System.Collections;

public class PersistentTerrainSettings : MonoBehaviour {

    public static PersistentTerrainSettings settings;

    public float sideLength = 100f;
    public float gravityEffect = 1f;
    public float frequency = 6.27f;
    [Range(1, 3)]
    public int dimensions = 3;
    public NoiseType noiseType = NoiseType.Perlin;
    [Range(1, 8)]
    public int octaves = 3;
    [Range(1, 4)]
    public float lacunarity = 2f;
    [Range(0f, 1f)]
    public float gain = 0.5f;
    public Vector3 gradientOrigin = Vector3.zero;
    public float height = 10f;
    public Vector3 rotation = Vector3.zero;
    public TerrainTextureType textureType = TerrainTextureType.Rocky;
    [Range(1, 50)]
    public int tileSize = 15;
    public Texture2D[] terrainTextures;
    [Range(0, 100)]
    public float treeDensity = 22.5f;
    public string seed = "0";

    public Vector3 terrainPosition = Vector3.zero;
    public int difficulty = 20;
    public int numEnemies = 3;

    public static float poisonAmount = 0.1f; //how much the player gets hurt from staying in a poison cloud

    void Awake () {
        if (settings == null) {
            SetDefault ();
            DontDestroyOnLoad (gameObject);
            settings = this;
        } else if (settings != this) {
            Destroy (gameObject);
        }
    }

    public void SetMediumDifficulty() {
        print ("Set Medium Difficulty");
        sideLength = 150f;
        height = 20f;
        poisonAmount = 0.3f;
    }

    public void SetHardDifficulty() {
        print ("Set Hard Difficulty");
        sideLength = 200f;
        height = 40f;
        poisonAmount = 0.5f;
    }

    // Temporary level loading mechanism, for now
    // int difficulty doesn't really have a meaning
    // TODO: is this still temporary? If so where should the logic go? JPC 11/2/15
    public void LoadLevelSettings() {
        print("Load level! :D");
        print(difficulty);

        height += 2; // increase the height a little bit every time this function is called.
        poisonAmount += 0.05f; //make poison clouds a little more dangerous

        if (difficulty > 30 && difficulty < 70) {
            textureType = TerrainTextureType.Grassy;
        } else if (difficulty >= 0 && difficulty < 30) {
            textureType = TerrainTextureType.Desert;
        } else {
            textureType = TerrainTextureType.Rocky;
        }
    }

    public void SetDefault()
    {
        print ("Set default terrain");
        sideLength = 100f;
        gravityEffect = 1f;
        frequency = 6.27f;
        dimensions = 3;
        noiseType = NoiseType.Perlin;
        octaves = 3;
        lacunarity = 2f;
        gain = 0.5f;
        gradientOrigin = Vector3.zero;
        height = 10f;
        rotation = Vector3.zero;
        textureType = TerrainTextureType.Rocky;
        tileSize = 15;
        treeDensity = 22.5f;
        terrainPosition = Vector3.zero;
        seed = "0";
        poisonAmount = 0.1f;
    }

    public void OnDestroy()
    {
        print ("Destroyed PersistentTerrainSettings");
        SetDefault ();

    }
}
