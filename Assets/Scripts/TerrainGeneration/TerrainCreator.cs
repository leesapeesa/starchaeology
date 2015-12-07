using UnityEngine;
using UnityEngine.Assertions;

public enum TerrainTextureType
{
    Desert,
    Grassy,
    Rocky
}

/// <summary>
/// TerrainCreator is responsible for procedurally generating the terrain and creating a 2d edge collider for it.
/// It includes multiple public parameters that may be adjusted to create radically different worlds.
/// </summary>
[RequireComponent(typeof(Terrain))]
public class TerrainCreator : MonoBehaviour
{
    // Eventually all of these will be private.
    private float sideLength;
    private float frequency;
    [Range(1, 3)]
    private int dimensions;
    private NoiseType noiseType = NoiseType.Perlin;
    [Range(1, 8)]
    private int octaves;
    [Range(1, 4)]
    private float lacunarity;
    [Range(0f, 1f)]
    private float gain;
    private Vector3 gradientOrigin;
    private float height;
    private Vector3 rotation;
    private TerrainTextureType textureType;
    [Range(1, 50)]
    private int tileSize;
    private Texture2D[] terrainTextures;
    [Range(0, 100)]
    private float treeDensity;
    private string seed;
    private bool useRandomSeed = false;
    private float triggerOffset = 50f;
    private float fallHeight;

    private float[,] heights;
    private Terrain terrain;
    private GameObject terrain2dCollider;
    private System.Random pseudoRandom;
    private GameObject triggerBounds;

    private const float REAL_HEIGHT = 100f; //what the engine sees as the height of the TerrainData

    private void Start()
    {
        SetAllOptions();

        terrain = GetComponent<Terrain>();
        Debug.Assert(terrain != null);
        terrain.terrainData.size = new Vector3(sideLength, REAL_HEIGHT, sideLength);
        terrain.terrainData.treeInstances = new TreeInstance[0];
        terrain2dCollider = GameObject.Find("Terrain2dCollider");
        Debug.Assert(terrain2dCollider != null);
        terrain2dCollider.AddComponent<EdgeCollider2D>();
        Refresh();
        PersistentTerrainSettings.settings.terrainPosition = transform.position;
        // If we need to save any options.
        SaveAllOptions ();
        // Now we need to place the gameObject that will kill off everything if it hits it.
        AddTriggerBounds ();
    }
    private void AddTriggerBounds() {
        fallHeight = height / 2;
        triggerBounds = GameObject.Find ("TriggerBounds");
        Debug.Assert (triggerBounds != null);
        // For now, set the height to be the same height as the terrain.
        triggerBounds.transform.localScale = new Vector3 (sideLength + triggerOffset, height, 0);
        triggerBounds.transform.position = transform.position + new Vector3 (sideLength / 2, -fallHeight, 0);
    }

    private void RandomOrigin() {
        if (useRandomSeed) {
            //If we are loading from a saved game, use the saved seed rather than a new one, so that we
            //generate the same terrain.
            if (PersistentLevelSettings.settings.loadFromSave)
                seed = PlayerPrefs.GetString("terrainSeed" + PersistentLevelSettings.settings.loadSlot);
            else
                seed = Time.time.ToString ();
        }
        pseudoRandom = new System.Random(seed.GetHashCode());
        gradientOrigin = new Vector3 (pseudoRandom.Next(0,100), pseudoRandom.Next(0,100), pseudoRandom.Next(0,100));
    }
    private void OnDestroy()
    {
        // On Destroy of PersistentTerrainSettings will set everything to its default value, so we
        // just want to rebuild everything with the default values so that we don't change the
        // terrain data every time we open it. 
        SetEverythingToZero ();

    }

    private void SetEverythingToZero() {
        sideLength = 0f;
        frequency = 0f;
        dimensions = 0;
        octaves = 0;
        lacunarity = 0f;
        gain = 0f;
        gradientOrigin = Vector3.zero;
        height = 0f;
        rotation = Vector3.zero;
        textureType = TerrainTextureType.Desert;
        tileSize = 0;
        treeDensity = 0f;
        transform.position = Vector3.zero;
        seed = "0";
        terrain.terrainData.treeInstances = new TreeInstance[0];
        heights = new float[terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution];
        terrain.terrainData.SetHeights(0, 0, heights);

        float[,,] alphas = terrain.terrainData.GetAlphamaps(0, 0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);
        terrain.terrainData.SetAlphamaps(0, 0, alphas);
    }

    private void SetAllOptions () {
        sideLength = PersistentTerrainSettings.settings.sideLength;
        frequency = PersistentTerrainSettings.settings.frequency;
        dimensions = PersistentTerrainSettings.settings.dimensions;
        octaves = PersistentTerrainSettings.settings.octaves;
        lacunarity = PersistentTerrainSettings.settings.lacunarity;
        gain = PersistentTerrainSettings.settings.gain;
        gradientOrigin = PersistentTerrainSettings.settings.gradientOrigin;
        height = PersistentTerrainSettings.settings.height;
        rotation = PersistentTerrainSettings.settings.rotation;
        textureType = PersistentTerrainSettings.settings.textureType;
        tileSize = PersistentTerrainSettings.settings.tileSize;
        terrainTextures = PersistentTerrainSettings.settings.terrainTextures;
        treeDensity = PersistentTerrainSettings.settings.treeDensity;
        transform.position = PersistentTerrainSettings.settings.terrainPosition;
        seed = PersistentTerrainSettings.settings.seed;
    }

    public void SaveAllOptions () {
        PersistentTerrainSettings.settings.sideLength = sideLength;
        PersistentTerrainSettings.settings.frequency = frequency;
        PersistentTerrainSettings.settings.dimensions = dimensions;
        PersistentTerrainSettings.settings.octaves = octaves;
        PersistentTerrainSettings.settings.lacunarity = lacunarity;
        PersistentTerrainSettings.settings.gain = gain;
        PersistentTerrainSettings.settings.gradientOrigin = gradientOrigin;
        PersistentTerrainSettings.settings.height = height;
        PersistentTerrainSettings.settings.rotation = rotation;
        PersistentTerrainSettings.settings.textureType = textureType;
        PersistentTerrainSettings.settings.tileSize = tileSize;
        PersistentTerrainSettings.settings.terrainTextures = terrainTextures;
        PersistentTerrainSettings.settings.treeDensity = treeDensity;
        PersistentTerrainSettings.settings.terrainPosition = transform.position;
        PersistentTerrainSettings.settings.seed = seed;
    }

    public void Refresh()
    {
        Generate();
        terrain.terrainData.treeInstances = new TreeInstance[0];

        Quaternion curRotation = Quaternion.Euler(rotation);
        //Generate a random map of height values
        //First, define the bounding box for the gradient. This determines what part of the sampling space we use
        Vector3 point00 = curRotation * (gradientOrigin + new Vector3(-0.5f, -0.5f));
        Vector3 point10 = curRotation * (gradientOrigin + new Vector3(0.5f, -0.5f));
        Vector3 point01 = curRotation * (gradientOrigin + new Vector3(-0.5f, 0.5f));
        Vector3 point11 = curRotation * (gradientOrigin + new Vector3(0.5f, 0.5f));

        NoiseFunction noiseFunction = Noise.noiseMethods[(int)noiseType][dimensions - 1];
        float stepSize = 1f / terrain.terrainData.heightmapResolution;
        //Ratio of desired terrain height to the actual height of the TerrainData
        //Needed so that the front-facing "wall" can extend below the bottom of the terrain, such that the user never sees the bottom
        float heightRatio = height / REAL_HEIGHT;
        //Compute the gradient value at each vertex of the surface.
        //Note: coordinates here are in gradient space, not local space
        for (int y = 0; y < terrain.terrainData.heightmapResolution; y++)
        {
            Vector3 pointLeft = Vector3.Lerp(point00, point01, y * stepSize);
            Vector3 pointRight = Vector3.Lerp(point10, point11, y * stepSize);
            for (int x = 0; x < terrain.terrainData.heightmapResolution; x++)
            {
                Vector3 point = Vector3.Lerp(pointLeft, pointRight, x * stepSize);
                float sample = Noise.Sum(noiseFunction, point, frequency, octaves, lacunarity, gain);
                if (noiseType != NoiseType.Value)
                    sample = sample * 0.5f + 0.5f;
                if (y == 0) //drop down the vertices facing the camera to create a front-facing wall for the 2d view
                    heights[y, x] = -100;
                else
                    heights[y, x] = heightRatio * (sample - 1) + 1;
            }
        }

        terrain.terrainData.SetHeights(0, 0, heights);
        transform.position = new Vector3(-sideLength / 2, -REAL_HEIGHT + height / 2, -sideLength / terrain.terrainData.heightmapResolution);

        terrain2dCollider.GetComponent<EdgeCollider2D>().points = getPathHeights();

        assignTexture((int)textureType);

        addTrees();
    }

    /// <summary>
    /// Get the heights of each vertex that lies on the path of the player character
    /// </summary>
    public Vector2[] getPathHeights()
    {
        Vector2[] result = new Vector2[terrain.terrainData.heightmapResolution];
        for (int x = 0; x < terrain.terrainData.heightmapResolution; x++)
            result[x] = new Vector2(x * sideLength / terrain.terrainData.heightmapResolution - sideLength / 2,
                                    terrain.terrainData.GetHeight(x, 1) - REAL_HEIGHT + height / 2);
        return result;
    }

    // Returns the resolution of the height map
    public int GetHeightmapResolution() {
        return terrain.terrainData.heightmapResolution;
    }

    /// <summary>
    /// Save any relevant terrain data that cannot be reconstructed from the saved difficulty
    /// </summary>
    public void SaveTerrain(int slotId)
    {
        PlayerPrefs.SetString("terrainSeed" + slotId, seed);
    }

    private void Generate()
    {
        terrain.terrainData.size = new Vector3(sideLength, REAL_HEIGHT, sideLength);
        heights = new float[terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution];

        RandomOrigin ();


        //Generate terrain texture maps (splatmaps) from the provided list of textures.
        Debug.Assert(terrainTextures != null);
        //For each texture, we need two maps: one for the surface and one for the camera-facing wall
        SplatPrototype[] splats = new SplatPrototype[terrainTextures.Length * 2];
        for (int i = 0; i < terrainTextures.Length; i++)
        {
            splats[i] = new SplatPrototype();
            splats[i].texture = terrainTextures[i];
            splats[i].tileSize = new Vector2(tileSize, tileSize);
        }
        for (int i = terrainTextures.Length; i < splats.Length; i++)
        {
            splats[i] = new SplatPrototype();
            splats[i].texture = terrainTextures[i - terrainTextures.Length];
            splats[i].tileSize = new Vector2(terrain.terrainData.alphamapResolution, 0.1f);
        }
        terrain.terrainData.splatPrototypes = splats;
    }

    /// <summary>
    /// Set the texture of the terrain
    /// </summary>
    private void assignTexture(int textureIndex)
    {
        float[,,] alphas = terrain.terrainData.GetAlphamaps(0, 0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);

        //Assign the desired texture to each grid on the terrain
        for (int i = 0; i < terrain.terrainData.alphamapWidth; i++)
        {
            for (int j = 0; j < terrain.terrainData.alphamapHeight; j++)
            {
                //Set the strength of the desired texture to maximum, while setting the strengths of all other textures to 0
                for (int tex = 0; tex < terrain.terrainData.alphamapLayers; tex++)
                {
                    alphas[i, j, tex] = (tex == textureIndex) ? 1 : 0;
                }
            }
        }

        //For the front-facing wall, use the 1x1 tiling version of the texture
        for (int j = 0; j < terrain.terrainData.alphamapHeight; j++)
        {
            for (int tex = 0; tex < terrain.terrainData.alphamapLayers; tex++)
            {
                alphas[0, j, tex] = ((tex - terrainTextures.Length) == textureIndex) ? 1 : 0;
                alphas[1, j, tex] = ((tex - terrainTextures.Length) == textureIndex) ? 1 : 0;
            }
        }

        terrain.terrainData.SetAlphamaps(0, 0, alphas);
    }

    /// <summary>
    /// Randomly distribute trees throughout the terrain
    /// </summary>
    private void addTrees()
    {
        for (int x = 1; x < sideLength; x++)
        {
            for (int z = 1; z < sideLength; z++)
            {
                //treeDensity represents expected value of total number of trees
                //Therefore, each unit square has a treeDensity / Area probability of having a tree
                if (Random.value < treeDensity / (sideLength * sideLength))
                {
                    TreeInstance newTree = new TreeInstance();
                    newTree.prototypeIndex = (int)textureType; //right now each terrain type has its own tree type
                    newTree.position = new Vector3((x / sideLength), 1, (z / sideLength));
                    newTree.heightScale = 0.3f;
                    newTree.widthScale = 0.3f;
                    newTree.rotation = Random.value * 2 * Mathf.PI;
                    terrain.AddTreeInstance(newTree);
                }
            }
        }
    }
}
