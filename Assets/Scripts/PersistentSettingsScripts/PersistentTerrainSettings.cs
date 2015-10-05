using UnityEngine;
using System.Collections;

public class PersistentTerrainSettings : MonoBehaviour {

    public float sideLength = 100f;
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

    void Awake () {
		SetDefault ();
        // Sets the game as the previous scene so that menus know what to load
        PlayerPrefs.SetInt("previousScene", Application.loadedLevel);
        DontDestroyOnLoad(transform.gameObject);
    }

    // Temporary level loading mechanism, for now 
    // int difficulty doesn't really have a meaning
    public void LoadLevelSettings(int difficulty) {
        print("Load level! :D");
        print(difficulty);

        if (difficulty > 30 && difficulty < 70) {
            textureType = TerrainTextureType.Grassy;
        } else if (difficulty >= 0 && difficulty < 30) {
            textureType = TerrainTextureType.Desert;
        } else {
            textureType = TerrainTextureType.Rocky;
        }
        Application.LoadLevel(Application.loadedLevel);
    }

	public void SetDefault()
	{
		print ("Set default terrain");
		sideLength = 100f;
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

	public void OnDestroy()
	{
		print ("Destroyed PersistentTerrainSettings");
		SetDefault ();

	}
}
