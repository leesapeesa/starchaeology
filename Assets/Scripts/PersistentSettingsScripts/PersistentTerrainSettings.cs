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
    public float height = 100f;
    public Vector3 rotation = Vector3.zero;
    public TerrainTextureType textureType = TerrainTextureType.Rocky;
    [Range(1, 50)]
    public int tileSize = 15;
    public Texture2D[] terrainTextures;
    [Range(0, 100)]
    public float treeDensity = 22.5f;
    public string seed = "0";

    public Vector3 terrainPosition = Vector3.zero;

    // Just so that when we quit the game, our prefabs don't change anymore
    // and cause annoying commit errors.
    public float DEFAULT_GRAVITY_EFFECT = 1f;

    void Awake () {
        if (settings == null) {
            SetDefault ();
            DontDestroyOnLoad (gameObject);
            settings = this;
        } else if (settings != this) {
            Destroy (gameObject);
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
    }

    /// <summary>
    /// Save any relevant terrain settings that cannot be reconstructed from the saved difficulty
    /// </summary>
    public void SaveTerrainSettings(int slotId)
    {
        PlayerPrefs.SetFloat("gravity" + slotId, gravityEffect);
        PlayerPrefs.SetInt("textureType" + slotId, (int)textureType);
    }

    public void LoadTerrainSettings(int slotId)
    {
        gravityEffect = PlayerPrefs.GetFloat("gravity" + slotId);
        textureType = (TerrainTextureType)PlayerPrefs.GetInt("textureType" + slotId);
    }

    public void OnDestroy()
    {
        print ("Destroyed PersistentTerrainSettings");
        SetDefault ();
    }
}
