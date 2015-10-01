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
    public float sideLength = 0f;
    public float frequency = 1f;
    [Range(1, 3)]
    public int dimensions = 3;
    public NoiseType noiseType;
    [Range(1, 8)]
    public int octaves = 2;
    [Range(1, 4)]
    public float lacunarity = 2f;
    [Range(0f, 1f)]
    public float gain = 0.5f;
    public Vector3 gradientOrigin = Vector3.zero;
    public float height = 1f;
    public Vector3 rotation = Vector3.zero;
    public TerrainTextureType textureType;
    [Range(1, 50)]
    public int tileSize = 15;
    public Texture2D[] terrainTextures;
    [Range(0, 100)]
    public float treeDensity = 1;

    private float[,] heights;
    private Terrain terrain;
    private float curHeight = 0;
    private float curLength = 0;
    private GameObject terrain2dCollider;
    private GameObject persistentTerrainSettings = null;
    private PersistentTerrainSettings settings;

    private void OnEnable()
    {
        persistentTerrainSettings = GameObject.FindWithTag("Settings"); 
        Assert.IsNotNull(persistentTerrainSettings);
        settings = persistentTerrainSettings.GetComponent<PersistentTerrainSettings>();
		persistentTerrainSettings.GetComponent<PersistentTerrainSettings>().SetDefault ();
        SetOptions();

        terrain = GetComponent<Terrain>();
        Debug.Assert(terrain != null);
        terrain.terrainData.size = new Vector3(sideLength, height, sideLength);
        terrain.terrainData.treeInstances = new TreeInstance[0];
        terrain2dCollider = GameObject.Find("Terrain2dCollider");
        Debug.Assert(terrain2dCollider != null);
        terrain2dCollider.AddComponent<EdgeCollider2D>();
        Refresh();
    }

	private void OnDestroy()
	{
		SetOptions ();
	}

    private void SetOptions () {
        sideLength = settings.sideLength;
        frequency = settings.frequency;
        dimensions = settings.dimensions;
        octaves = settings.octaves;
        lacunarity = settings.lacunarity;
        gain = settings.gain;
        gradientOrigin = settings.gradientOrigin;
        height = settings.height;
        rotation = settings.rotation;
        textureType = settings.textureType;
        tileSize = settings.tileSize;
        terrainTextures = settings.terrainTextures;
        treeDensity = settings.treeDensity;
    }

    public void Refresh()
    {
        Generate();

        Quaternion curRotation = Quaternion.Euler(rotation);
        //Generate a random map of height values
        //First, define the bounding box for the gradient. This determines what part of the sampling space we use
        Vector3 point00 = curRotation * (gradientOrigin + new Vector3(-0.5f, -0.5f));
        Vector3 point10 = curRotation * (gradientOrigin + new Vector3(0.5f, -0.5f));
        Vector3 point01 = curRotation * (gradientOrigin + new Vector3(-0.5f, 0.5f));
        Vector3 point11 = curRotation * (gradientOrigin + new Vector3(0.5f, 0.5f));

        NoiseFunction noiseFunction = Noise.noiseMethods[(int)noiseType][dimensions - 1];
        float stepSize = 1f / terrain.terrainData.heightmapResolution;
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
                    heights[y, x] = sample;
            }
        }

        terrain.terrainData.SetHeights(0, 0, heights);
        transform.position = new Vector3(-sideLength / 2, -height / 2, -sideLength / terrain.terrainData.heightmapResolution);

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
                                    terrain.terrainData.GetHeight(x, 1) - height / 2);
        return result;
    }

    private void Generate()
    {
        //update parameters for dynamic updating in debug mode
        curLength = sideLength;
        curHeight = height;

        terrain.terrainData.size = new Vector3(sideLength, height, sideLength);
        heights = new float[terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution];

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
            splats[i].tileSize = new Vector2(terrain.terrainData.alphamapResolution, 1);
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
