using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Terrain))]
public class TerrainCreator : MonoBehaviour
{
    public float sideLength = 5f;
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

    private float[,] heights;
    private Terrain terrain;
    private float curHeight = 0;
    private float curLength = 0;

    private void OnEnable()
    {
        terrain = GetComponent<Terrain>();
        terrain.terrainData.size = new Vector3(sideLength, height, sideLength);
        Refresh();
    }

    public void Refresh()
    {
        if (sideLength != curLength || height != curHeight)
            Generate();
        Quaternion curRotation = Quaternion.Euler(rotation);
        //Generate a random map of height values
        //First, define the bounding box for the gradient. This determines what part of the sampling space we use
        Vector3 point00 = curRotation * gradientOrigin + new Vector3(-0.5f, -0.5f);
        Vector3 point10 = curRotation * gradientOrigin + new Vector3(0.5f, -0.5f);
        Vector3 point01 = curRotation * gradientOrigin + new Vector3(-0.5f, 0.5f);
        Vector3 point11 = curRotation * gradientOrigin + new Vector3(0.5f, 0.5f);

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
                if (y == 0)
                    heights[y, x] = -10;
                heights[y, x] = sample;
            }
        }

        terrain.terrainData.SetHeights(0, 0, heights);
    }

    private void Generate()
    {
        //update parameters for dynamic updating in debug mode
        curLength = sideLength;
        curHeight = height;

        terrain.terrainData.size = new Vector3(sideLength, height, sideLength);
        heights = new float[terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution];
    }
}
