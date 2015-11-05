using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour {
    //TODO make all these be modified by Difficulty - JPC 11/5/15
    public int boxCount = 5;
    public int collectCount = 10;
    public int slowCloudCount = 5;
    public int poisonCloudCount = 4;
    public int enemyCount = 3;
    public float unreachableFactor = 5f;
    public int jumpPlatformCount = 5;

    public bool allCollected = false;

    public Transform bouncyBox;
    public Transform stickyBox;
    public Transform slowCloud;
    public Transform poisonCloud;
    public Transform jumpPlatform;
    public Transform[] enemyList;
    public Transform[] collectibleList;

    private TerrainCreator terrainCreator;
    private Vector2[] heights;

    private float sideLength = 25f;
    private float closestToEdge = 5f;
    private List<Transform> collectibles;
    private float gravityEffect;
    private Dictionary<Transform, float> pTable; //enemy type probability table

    // Use this for initialization
    void Start() {
        terrainCreator = GameObject.FindObjectOfType<TerrainCreator> ();
        heights = terrainCreator.getPathHeights();
        sideLength = PersistentTerrainSettings.settings.sideLength - closestToEdge;
        gravityEffect = PersistentTerrainSettings.settings.gravityEffect;
        enemyCount = PersistentLevelSettings.settings.numEnemies;
        slowCloudCount = PersistentLevelSettings.settings.numSlowClouds;
        poisonCloudCount = PersistentLevelSettings.settings.numPoisonClouds;

        //initialize the probability table
        pTable = new Dictionary<Transform, float>(enemyList.Length);
        pTable[enemyList[0]] = 0.8f;
        pTable[enemyList[1]] = 0.2f;

        GameObject newStickyBox = stickyBox.gameObject;
        Rigidbody2D stickyBoxRigidBody = newStickyBox.GetComponent<Rigidbody2D>();
        stickyBoxRigidBody.gravityScale = gravityEffect;

        GameObject newBouncyBox = bouncyBox.gameObject;
        Rigidbody2D bouncyBoxRigidBody = newBouncyBox.GetComponent<Rigidbody2D>();
        bouncyBoxRigidBody.gravityScale = gravityEffect;

        foreach (Transform enemy in enemyList) {
            GameObject newEnemy = enemy.gameObject;
            Rigidbody2D enemyRigidbody = newEnemy.GetComponent<Rigidbody2D>();
            enemyRigidbody.gravityScale = gravityEffect;
        }

        collectibles = new List<Transform> ();
        addCollectibles ();
        addObjects(bouncyBox, boxCount, 2);
        addObjects(slowCloud, slowCloudCount, 2);
        addObjects(stickyBox, boxCount, 5);
        addObjects(poisonCloud, poisonCloudCount, 2);
        addObjects(jumpPlatform, jumpPlatformCount, apex ());
        addEnemies(enemyCount);
    }

    void Update() {
        if (!collectibles.Any ()) {
            allCollected = true;
        }
    }

    private void addCollectibles() {
        // PathHeights is of length HeightmapResolution and corresponds to an actual
        // index by index * SideLength / HeightmapResolution.
        heights = terrainCreator.getPathHeights();
        for (int i = 0; i < collectCount; ++i) {
            Vector2 randomPointOnTerrain = GetRandomPointOnTerrain();

            float maxHeight = randomPointOnTerrain.y + apex();
            // Let the possibility of genereting a few collectibles slightly out of reach.
            float height = Random.Range(randomPointOnTerrain.y, maxHeight);
            Vector3 position = new Vector3(randomPointOnTerrain.x, height + unreachableFactor) ;
            Transform collect = collectibleList[Random.Range(0, collectibleList.Length)].transform;
            collectibles.Add (Instantiate(collect, position, Quaternion.identity) as Transform);
        }
    }

    // From Path Height Index to Screen Coordinates
    private Vector2 GetRandomPointOnTerrain() {
        int heightmapResolution = terrainCreator.GetHeightmapResolution();
        float index = Random.Range(0, heightmapResolution);

        float yCoor = heights[(int) index].y;
        float xCoor = (int) index * (float) sideLength / heightmapResolution;
        return new Vector2 (xCoor - sideLength / 2, yCoor);
    }

    private Vector3 GetRandomPointAboveTerrain(float height) {
        Vector2 pos = GetRandomPointOnTerrain ();
        return new Vector3 (pos.x, Random.Range (pos.y, pos.y + height), 0f);
    }

    // The maximum reachable height that the player can jump to.
    private float apex() {
        // Simple mechanics: maxHeight = v_0^2 / (2 * g) + y_0
        // where v_0 = (jumpForce - gravityEffect), g = gravityEffect and y_0 = terrainHeight.
        float approxLengthOfOneFrame = Time.fixedDeltaTime;
        float gravity = PersistentTerrainSettings.settings.gravityEffect * 6f;
        float initialVelocity = (PersistentPlayerSettings.settings.jumpForce - gravity) * approxLengthOfOneFrame;

        return initialVelocity * initialVelocity / (2 * gravity);
    }

    private void addObjects(Transform obj, int count, float height = 2f)
    {
        for (int i = 0; i < count; ++i) {
            Vector3 position = GetRandomPointAboveTerrain(height);
            Instantiate(obj, position, Quaternion.identity);
        }
    }

    /// <summary>
    /// Add enemies to the scene, randomly choosing the enemy type based on the current probability table.
    /// </summary>
    private void addEnemies(int count)
    {
        for (int i = 0; i < count; ++i) {
            float pValue = Random.value;
            float prior = 0;
            foreach (Transform enemy in enemyList) {
                if (pValue < pTable[enemy] + prior) {
                    Vector3 position = GetRandomPointAboveTerrain(unreachableFactor);
                    position = position + new Vector3(0, 5f, 0);
                    Instantiate(enemy, position, Quaternion.identity);
                    prior += pTable[enemy];
                    break;
                }
                prior += pTable[enemy];
            }
        }
    }

    public void RemoveFromScene(NonPlayerObject npo) {
        if (collectibles.Contains (npo.transform))
            collectibles.Remove (npo.transform);
    }
}
