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

    [SerializeField] private Transform bouncyBox;
    [SerializeField] private Transform stickyBox;
    [SerializeField] private Transform slowCloud;
    [SerializeField] private Transform poisonCloud;
    [SerializeField] private Transform jumpPlatform;
    [SerializeField] private Transform[] enemyList;
    [SerializeField] private Transform[] collectibleList;
    [SerializeField] private Transform specialItem;
    [SerializeField] private Transform spaceship;

    private TerrainCreator terrainCreator;
    private Vector2[] heights;

    private float sideLength = 25f;
    private float closestToEdge = 5f;
    private List<Transform> collectibles;
    private List<Transform> specialItems;
    private float gravityEffect;
    private Dictionary<Transform, float> pTable; //enemy type probability table

    private const float PLATFORM_HEIGHT = 0.6f; //The platform height
    private const float PLATFORM_LENGTH = 2.5f;
    private const float MIN_PLATFORM_HEIGHT = 1.25f; // Minimum crouching height
    private const float SPACESHIP_POSITION = 6f;
    private const float SPACESHIP_HEIGHT = 6.8f;
    private const float SPACESHIP_DEPTH = 3.25f;
    private const float RAYCAST_ORIGIN = 1000;
    private const int POINT_COLLECTIBLE_INDEX = 0; //index of the point collectible in the collectibles list

    // Use this for initialization
    void Start() {
        terrainCreator = GameObject.FindObjectOfType<TerrainCreator> ();
        heights = terrainCreator.getPathHeights();
        sideLength = PersistentTerrainSettings.settings.sideLength - closestToEdge;
        gravityEffect = PersistentTerrainSettings.settings.gravityEffect;
        enemyCount = PersistentLevelSettings.settings.numEnemies;
        slowCloudCount = PersistentLevelSettings.settings.numSlowClouds;
        poisonCloudCount = PersistentLevelSettings.settings.numPoisonClouds;
        collectCount = PersistentLevelSettings.settings.collectCount;

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

    // Since we change the prefabs, we need to make sure to change it back.
    void OnDestroy() {
        GameObject newStickyBox = stickyBox.gameObject;
        Rigidbody2D stickyBoxRigidBody = newStickyBox.GetComponent<Rigidbody2D>();
        stickyBoxRigidBody.gravityScale = PersistentTerrainSettings.settings.DEFAULT_GRAVITY_EFFECT;
        
        GameObject newBouncyBox = bouncyBox.gameObject;
        Rigidbody2D bouncyBoxRigidBody = newBouncyBox.GetComponent<Rigidbody2D>();
        bouncyBoxRigidBody.gravityScale = PersistentTerrainSettings.settings.DEFAULT_GRAVITY_EFFECT;
        foreach (Transform enemy in enemyList) {
            GameObject newEnemy = enemy.gameObject;
            Rigidbody2D enemyRigidbody = newEnemy.GetComponent<Rigidbody2D>();
            enemyRigidbody.gravityScale = PersistentTerrainSettings.settings.DEFAULT_GRAVITY_EFFECT;
        }
    }

    /// <summary>
    /// Add items to the scene based on the current parameters and given objective.
    /// In order to ensure that all pertinent level information is loaded prior to item initialization,
    /// this function MUST be called at the end of LevelScript's Start routine.
    /// </summary>
    public void InitializeItems(Objective objective)
    {
        //First, make sure we will have enough items to complete the objective
        collectCount = Mathf.Max(collectCount, objective.NumSpecialItems, objective.NumPointItems);

        addCollectibles(objective);
        addObjects(bouncyBox, boxCount, 2);
        addObjects(slowCloud, slowCloudCount, 2);
        addObjects(stickyBox, boxCount, 5);
        addObjects(poisonCloud, poisonCloudCount, 2);
        addPlatforms(jumpPlatform, jumpPlatformCount, apex() - PLATFORM_HEIGHT);
        addEnemies(enemyCount);
        addSpaceship();
        maybeAddSpecialItems(objective);
    }

    private void addCollectibles(Objective objective) {
        // PathHeights is of length HeightmapResolution and corresponds to an actual
        // index by index * SideLength / HeightmapResolution.
        heights = terrainCreator.getPathHeights();
        int numPointCollectiblesAdded = 0;
        for (int i = 0; i < collectCount; ++i) {
            Vector2 randomPointOnTerrain;
            GetRandomPointOnWalkable(out randomPointOnTerrain);

            //ensure that enough point collectibles are spawned before spawning any other types
            int itemIndex = numPointCollectiblesAdded < objective.NumPointItems ?
                            POINT_COLLECTIBLE_INDEX :
                            Random.Range(0, collectibleList.Length);

            //update how many point collectibles have been added
            if (itemIndex == POINT_COLLECTIBLE_INDEX)
                ++numPointCollectiblesAdded;

            float maxHeight = randomPointOnTerrain.y + apex();
            // Let the possibility of genereting a few collectibles slightly out of reach.
            float height = Random.Range(randomPointOnTerrain.y, maxHeight);
            Vector3 position = new Vector3(randomPointOnTerrain.x, height + unreachableFactor) ;
            Transform collect = collectibleList[itemIndex].transform;
            collectibles.Add (Instantiate(collect, position, Quaternion.identity) as Transform);
        }
    }

    // From Path Height Index to Screen Coordinates
    // returns the point by reference.
    private void GetRandomPointOnWalkable(out Vector2 point) {
        float xCoor = Random.Range(-sideLength / 2, sideLength / 2);
        float yCoor = 1000f;
        // We're casting a ray to see if it hits any object. Once it hits an object,
        // that will be our new "height" at that point in the terrain. In this way
        // we can generate items above platforms.
        RaycastHit2D hit = Physics2D.Raycast (new Vector2 (xCoor, yCoor), 
                                              -Vector2.up,
                                              distance: Mathf.Infinity,
                                              layerMask: LayerMask.GetMask(new string[] { "Ground", "Platform" }));
        if (hit.collider != null) {
            point = hit.point;
        } else {
            point = new Vector2 (xCoor, 10f);
        }
    }

    // Helps us not generate platforms in the ground.
    private void GetRandomPointStrictlyAboveWalkable (float minHeight, float maxHeight,
                                                     out Vector2 pos) {
        GetRandomPointOnWalkable (out pos);
        pos = new Vector2 (pos.x, Random.Range(pos.y + minHeight, pos.y + maxHeight));
    }

    private void GetRandomPointAboveWalkable(float height, out Vector2 pos) {
        GetRandomPointStrictlyAboveWalkable (0, height, out pos);
    }

    // The maximum reachable height that the player can jump to.
    private float apex() {
        // Simple mechanics: maxHeight = v_0^2 / (2 * g) + y_0
        // where v_0 = (jumpForce - gravityEffect), g = gravityEffect and y_0 = terrainHeight.
        float approxLengthOfOneFrame = Time.fixedDeltaTime;
        float gravity = PersistentTerrainSettings.settings.gravityEffect * 9.8f;
        float initialVelocity = (PersistentPlayerSettings.settings.jumpForce - gravity) * approxLengthOfOneFrame;

        return initialVelocity * initialVelocity / (2 * gravity);
    }

    private void addObjects(Transform obj, int count, float height = 2f)
    {
        for (int i = 0; i < count; ++i) {
            Vector2 position;
            GetRandomPointAboveWalkable(height, out position);
            Instantiate(obj, position, Quaternion.identity);
        }
    }

    private void addPlatforms(Transform obj, int count, float height) {
        for (int i = 0; i < count; ++i) {
            Vector2 position;
            GetRandomPointStrictlyAboveWalkable(MIN_PLATFORM_HEIGHT, height, out position);
            // Whatever point we just found, with our raycast will be the very left bottom corner
            // of the platform.
            position.x = position.x + PLATFORM_LENGTH / 2;
            position.y = position.y + PLATFORM_HEIGHT / 2;

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
                    GetRandomPointAboveWalkable(unreachableFactor, out position);
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
            Vector2 maxPos = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
            for (float x = -sideLength / 2; x < sideLength / 2; ++x) {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(x, RAYCAST_ORIGIN), 
                                                     Vector2.down, 
                                                     distance: Mathf.Infinity,
                                                     layerMask: LayerMask.GetMask(new string[] { "Ground", "Platform" }));
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

    /// <summary>
    /// Add the player's spaceship to the scene
    /// </summary>
    private void addSpaceship()
    {
        float xPos = -sideLength / 2 + SPACESHIP_POSITION;
        //Get the y coordinate at the "landing site"
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(xPos, RAYCAST_ORIGIN, SPACESHIP_DEPTH), Vector3.down, out hit)) {
            if (hit.collider != null) {
                Vector2 spaceshipPosition = new Vector2(xPos, hit.point.y + SPACESHIP_HEIGHT / 2);
                Instantiate(spaceship, spaceshipPosition, Quaternion.identity);
                movePlayer (spaceshipPosition);
            }
        }
    }

    // Moves the player's starting position to be right next to the spaceship.
    private void movePlayer(Vector2 position) {
        GameObject.Find ("Player").transform.position = position;
    }

    public void RemoveFromScene(NonPlayerObject npo) {
        if (collectibles.Contains (npo.transform))
            collectibles.Remove (npo.transform);
        if (specialItems.Contains(npo.transform))
            specialItems.Remove(npo.transform);
    }

    public int GetSpecialItemsRemaining()
    {
        return specialItems.Count;
    }
}
