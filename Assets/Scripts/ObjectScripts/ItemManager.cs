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
    public float unreachableFactor = 3f;
    public int jumpPlatformCount = 5;

    public bool allCollected = false;

    public Transform bouncyBox;
    public Transform stickyBox;
    public Transform slowCloud;
    public Transform poisonCloud;
    public Transform jumpPlatform;
    public Transform[] enemyList;
    public Transform[] collectibleList;
    public Transform specialItem;

    private TerrainCreator terrainCreator;
    private Vector2[] heights;

    private float sideLength = 25f;
    private float closestToEdge = 5f;
    private List<Transform> collectibles;
    private List<Transform> specialItems;
    private float gravityEffect;
    private Dictionary<Transform, float> pTable; //enemy type probability table

    private const float PLATFORM_HEIGHT_OFFSET = 5f; //half the platform height

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

        collectibles = new List<Transform>();
        specialItems = new List<Transform>();
    }

    void Update() {
        if (!collectibles.Any ()) {
            allCollected = true;
        }
    }

    /// <summary>
    /// Add items to the scene based on the current parameters and given objective.
    /// In order to ensure that all pertinent level information is loaded prior to item initialization,
    /// this function MUST be called at the end of LevelScript's Start routine.
    /// </summary>
    public void InitializeItems(Objective objective)
    {
        addCollectibles();
        addObjects(bouncyBox, boxCount, 2);
        addObjects(slowCloud, slowCloudCount, 2);
        addObjects(stickyBox, boxCount, 5);
        addObjects(poisonCloud, poisonCloudCount, 2);
        addObjects(jumpPlatform, jumpPlatformCount, apex() - PLATFORM_HEIGHT_OFFSET);
        addEnemies(enemyCount);
        maybeAddSpecialItems(objective);
    }

    private void addCollectibles() {
        // PathHeights is of length HeightmapResolution and corresponds to an actual
        // index by index * SideLength / HeightmapResolution.
        heights = terrainCreator.getPathHeights();
        for (int i = 0; i < collectCount; ++i) {
            Vector2 randomPointOnTerrain;
            GetRandomPointOnTerrain(out randomPointOnTerrain, "Platform");

            float maxHeight = randomPointOnTerrain.y + apex();
            // Let the possibility of genereting a few collectibles slightly out of reach.
            float height = Random.Range(randomPointOnTerrain.y, maxHeight);
            Vector3 position = new Vector3(randomPointOnTerrain.x, height + unreachableFactor) ;
            Transform collect = collectibleList[Random.Range(0, collectibleList.Length)].transform;
            collectibles.Add (Instantiate(collect, position, Quaternion.identity) as Transform);
        }
    }

    // From Path Height Index to Screen Coordinates
    // returns the point by reference. Also has a tag that you can check to make sure you don't
    // hit and returns if the tag was hit.
    private bool GetRandomPointOnTerrain(out Vector2 point, string tag = "TerrainCollider") {
        float xCoor = Random.Range(-sideLength / 2, sideLength / 2);
        float yCoor = 1000f;
        // We're casting a ray to see if it hits any object. Once it hits an object,
        // that will be our new "height" at that point in the terrain. In this way
        // we can generate items above platforms.
        RaycastHit2D hit = Physics2D.Raycast (new Vector2 (xCoor, yCoor), -Vector2.up);
        if (hit.collider != null) {
            point = hit.point;
            print("tag hit is " + hit.transform.gameObject.tag + " checking tag: " + tag);

            // If what is hit is another object we're checking, return true.
            if (hit.transform.gameObject.CompareTag(tag)) {
                print ("hit :" + tag);
                return true;
            }
        } else {
            point = new Vector2 (xCoor, 10f);
        }
        print ("returning false");
        return false;
    }

    private bool GetRandomPointAboveTerrain(float height, out Vector2 pos, string tag = "TerrainCollider") {
        bool hitNonTerrain = GetRandomPointOnTerrain (out pos, tag);
        pos = new Vector2 (pos.x, Random.Range (pos.y, pos.y + height));
        return hitNonTerrain;
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
            Vector2 position;
            GetRandomPointAboveTerrain(height, out position);
            Instantiate(obj, position, Quaternion.identity);
        }
    }

    private void addPlatforms(Transform obj, int count, float height) {
        for (int i = 0; i < count; ++i) {
            Vector2 position;
            while (GetRandomPointAboveTerrain(height, out position, "Platform"));
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
                    Vector2 position;
                    GetRandomPointAboveTerrain(unreachableFactor, out position);
                    position = position + new Vector2(0, 5f);
                    Instantiate(enemy, position, Quaternion.identity);
                    prior += pTable[enemy];
                    break;
                }
                prior += pTable[enemy];
            }
        }
    }

    /// <summary>
    /// Possibly add special items to the scene depending on the given objective
    /// </summary>
    private void maybeAddSpecialItems(Objective objective)
    {
        for (int i = 0; i < objective.NumSpecialItems; ++i) {
            Vector2 maxPos = Vector2.zero;
            for (float x = -sideLength / 2; x < sideLength / 2; ++x) {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(0, 1000), Vector2.down);
                if (hit.collider != null) {
                    Vector2 hitPos = hit.point;
                    if (hitPos.y > maxPos.y)
                        maxPos = hitPos;
                }
            }
            maxPos.y += apex();
            specialItems.Add(Instantiate(specialItem, maxPos, Quaternion.identity) as Transform);
        }
    }

    public void RemoveFromScene(NonPlayerObject npo) {
        if (collectibles.Contains (npo.transform))
            collectibles.Remove (npo.transform);
        if (specialItems.Contains(npo.transform))
            specialItems.Remove(npo.transform);
    }

    public int GetSpecialItemsRemaining()
    {
        print(specialItems.Count);
        return specialItems.Count;
    }
}
