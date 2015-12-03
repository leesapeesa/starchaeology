using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour {
    //TODO make all these be modified by Difficulty - JPC 11/5/15
    public int boxCount = 5;
    public int collectCount = 10;
    public int goalCollectCount = 5;
    public int slowCloudCount = 5;
    public int poisonCloudCount = 4;
    public int enemyCount = 3;
    public float unreachableFactor = 3f;
    public int jumpPlatformCount = 5;
    public int stackHeight = 10; // Max number of platforms in a stack

    public bool allCollected = false;

    [SerializeField] private Transform bouncyBox;
    [SerializeField] private Transform stickyBox;
    [SerializeField] private Transform slowCloud;
    [SerializeField] private Transform poisonCloud;
    [SerializeField] private Transform jumpPlatform;
    [SerializeField] private Transform[] enemyList;
    [SerializeField] private Transform[] collectibleList;
    [SerializeField] private Transform[] goalCollectibleList;
    [SerializeField] private Transform specialItem;
    [SerializeField] private Transform spaceship;

    private TerrainCreator terrainCreator;
    private Vector2[] heights;

    private float sideLength = 25f;
    private float closestToEdge = 5f;
    private List<Transform> collectibles;
    private List<Transform> specialItems;
    private List<Transform> clouds;
    private List<Transform> platforms;
    private List<Transform> enemies;
    private List<Transform> obstacles;
    private List<Transform> goalCollectibles;
    private float gravityEffect;
    private Dictionary<Transform, float> pTable; //enemy type probability table
    private int? goalItemType = null;

    private const float PLATFORM_HEIGHT = 1.0f; //The platform height
    private const float PLATFORM_LENGTH = 5.12f;
    private const float MIN_PLATFORM_HEIGHT = 3.0f; //1.25f; // Minimum crouching height
    private float MAX_PLATFORM_HEIGHT = 10.0f; 
    private const float SPACESHIP_POSITION = 6f;
    private const float SPACESHIP_HEIGHT = 6.8f;
    private const float SPACESHIP_DEPTH = 3.25f;
    private const float RAYCAST_ORIGIN = 1000;
    private const int POINT_COLLECTIBLE_INDEX = 0; //index of the point collectible in the collectibles list
    private const float LOAD_GAME_HEIGHT_OFFSET = 0.5f; //start things slightly above ground on load, so they don't get stuck in ground
    private const float MIN_ITEM_HEIGHT = 2;

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
        jumpPlatformCount = PersistentLevelSettings.settings.numPlatforms;
        goalCollectCount = PersistentLevelSettings.settings.goalCollectCount;

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
        clouds = new List<Transform>();
        platforms = new List<Transform>();
        enemies = new List<Transform>();
        obstacles = new List<Transform>();
        goalCollectibles = new List<Transform>();
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

        //If we are starting a level normally, randomly initialize all items. Otherwise, restore the items
        //from saved parameters.
        if (!PersistentLevelSettings.settings.loadFromSave) {
            MAX_PLATFORM_HEIGHT = apex() - PLATFORM_HEIGHT - 0.3f;
            if (MAX_PLATFORM_HEIGHT < MIN_PLATFORM_HEIGHT) {
                MAX_PLATFORM_HEIGHT = MIN_PLATFORM_HEIGHT;
            }
            MAX_PLATFORM_HEIGHT = MIN_PLATFORM_HEIGHT;
            addPlatforms(jumpPlatform, jumpPlatformCount); // Make the platforms slightly lower than the maximum jumping distance
            addCollectibles(objective);
            addObjects(bouncyBox, boxCount, obstacles, MIN_ITEM_HEIGHT);
            addObjects(slowCloud, slowCloudCount, clouds, MIN_ITEM_HEIGHT);
            addObjects(stickyBox, boxCount, obstacles, MIN_ITEM_HEIGHT);
            addObjects(poisonCloud, poisonCloudCount, clouds, MIN_ITEM_HEIGHT);
            addEnemies(enemyCount);
            maybeAddSpecialItems(objective);
            maybeAddGoalItems(objective, goalCollectCount);
        } else {
            int slotId = PersistentLevelSettings.settings.loadSlot;
            RestoreCollectibles(slotId);
            RestoreSpecialItems(slotId);
            RestoreObstacles(slotId);
            RestoreClouds(slotId);
            RestorePlatforms(slotId);
            RestoreEnemies(slotId);
            RestoreGoalItems(slotId);
            float playerX = PlayerPrefs.GetFloat("playerx" + slotId);
            float playerY = PlayerPrefs.GetFloat("playery" + slotId);
            movePlayer(new Vector2(playerX, playerY + LOAD_GAME_HEIGHT_OFFSET));
        }
        addSpaceship();
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

    /// <summary>
    /// Gets a point randomly on the map on a platform or the terrain. If there is a platform,
    /// it will return the value on the platform.
    /// </summary>
    /// <param name="point">Point.</param>
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

    /// <summary>
    ///  Takes in a point and finds a second point that is a distance xDistance away on the walkable surface from it
    /// </summary>
    /// <param name="firstPoint"></param>
    /// <param name="xDistance"></param>
    /// <param name="secondPoint"></param>
    private void GetPointAtDistanceOnWalkable(Vector2 firstPoint, float xDistance, out Vector2 secondPoint) {
        float xCoor = firstPoint.x + xDistance;
        float yCoor = 1000f;
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(xCoor, yCoor),
                                             -Vector2.up,
                                             distance: Mathf.Infinity,
                                             layerMask: LayerMask.GetMask(new string[] { "Ground", "Platform" }));
        if (hit.collider != null) {
            secondPoint = hit.point;
        } else {
            secondPoint = new Vector2(xCoor, 10f);
        }                                     
    }

    /// <summary>
    /// Gets a point in a range above a walkable surface.
    /// </summary>
    /// <param name="minHeight">Minimum height.</param>
    /// <param name="maxHeight">Max height.</param>
    /// <param name="pos">Position.</param>
    private void GetRandomPointStrictlyAboveWalkable (float minHeight, float maxHeight,
                                                     out Vector2 pos) {
        GetRandomPointOnWalkable (out pos);
        pos = new Vector2 (pos.x, Random.Range(pos.y + minHeight, pos.y + maxHeight));
    }
    /// <summary>
    /// Gets two points at the same height at distance apart from each other
    /// </summary>
    /// <param name="minHeight"></param>
    /// <param name="maxHeight"></param>
    /// <param name="leftPoint"></param>
    /// <param name="rightPoint"></param>
    private void GetTwoPointsStrictlyAboveWalkable (float distance, bool isRandom, float xCoord, out Vector2 leftPoint, out Vector2 rightPoint) {
        
        // Choose the point deterministically or randomly depending on input
        if (isRandom) {
            GetRandomPointOnWalkable(out leftPoint);
        } else {
            GetPointOnWalkable(xCoord, out leftPoint);
        }

        GetPointAtDistanceOnWalkable(leftPoint, distance, out rightPoint);

        float leftWalkableHeight = leftPoint.y;
        float rightWalkableHeight = rightPoint.y;

        if (Mathf.Abs(leftWalkableHeight - rightWalkableHeight) > MIN_PLATFORM_HEIGHT/3f) {
            GetTwoPointsStrictlyAboveWalkable(distance, false, leftPoint.x + 5.0f, out leftPoint, out rightPoint);
            return;
        }

        // We don't want platforms to spawn close to the edges of the terrain
        if (xCoord > (sideLength/2 - 5f) || xCoord < (-sideLength/2 + 5f)) {
            GetTwoPointsStrictlyAboveWalkable(distance, false, leftPoint.x / 2.0f, out leftPoint, out rightPoint);
            return;
        }

        // the right corner is closer to the ground, the left corner is farther from the ground
        if (leftPoint.y > rightPoint.y) {
            leftPoint = new Vector2(leftPoint.x, Random.Range(rightPoint.y + MIN_PLATFORM_HEIGHT, rightPoint.y + MAX_PLATFORM_HEIGHT));
        } else { // the left corner is closer to the ground, the right corner is farther from the ground
            leftPoint = new Vector2(leftPoint.x, Random.Range(leftPoint.y + MIN_PLATFORM_HEIGHT, leftPoint.y + MAX_PLATFORM_HEIGHT));
        }

        rightPoint = new Vector2(rightPoint.x, leftPoint.y);
        print(rightPoint.y - rightWalkableHeight);
        print(leftPoint.y - leftWalkableHeight);
    }

    private void GetPointOnWalkable(float xCoord, out Vector2 leftPoint) {
        float yCoor = 1000f;
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(xCoord, yCoor),
                                             -Vector2.up,
                                             distance: Mathf.Infinity,
                                             layerMask: LayerMask.GetMask(new string[] { "Ground", "Platform" }));
        if (hit.collider != null) {
            leftPoint = hit.point;
        } else {
            leftPoint = new Vector2(xCoord, 10f);
        }
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

    private void addObjects(Transform obj, int count, List<Transform> objectList, float height = 2f)
    {
        for (int i = 0; i < count; ++i) {
            Vector2 position;
            GetRandomPointAboveWalkable(height, out position);
            objectList.Add(Instantiate(obj, position, Quaternion.identity) as Transform);
        }
    }

    private void addPlatforms(Transform obj, int count) {
        for (int i = 0; i < count; ++i) {
            Vector2 leftPoint, rightPoint;
            GetTwoPointsStrictlyAboveWalkable(PLATFORM_LENGTH, true, 0.0f, out leftPoint, out rightPoint);

            bool validLocation = checkIfPlatformTooClose(leftPoint, 3.0f);
            if (!validLocation) { // We don't want to spawn platforms inside of other platforms
                continue;
            }

            // Take the leftmost point calculated (both should be at the same height)
            leftPoint.x = leftPoint.x + PLATFORM_LENGTH / 2;
            leftPoint.y = leftPoint.y + PLATFORM_HEIGHT / 2;

            if (leftPoint.x >= 50) {
                print("BAD PLATFORM!!!");
            }

            float rand = Random.Range(0, stackHeight);
            // add stacks randomly
            addPlatformStack(obj, (int)rand, leftPoint.y, leftPoint.x);

            platforms.Add(Instantiate(obj, leftPoint, Quaternion.identity) as Transform);
        }
    }

    private void addPlatformStack(Transform obj, int count, float height, float xLoc) {
        float whichLoc = Random.Range(0f, 1f);

        float onePosXLoc = Random.Range(xLoc - 7f, xLoc - 3f);
        float otherPosXLoc = Random.Range(xLoc + 3f, xLoc + 7f);

        float newXLoc;

        if (whichLoc < 0.5) {
            newXLoc = onePosXLoc;
        } else {
            newXLoc = otherPosXLoc;
        }

        float newYLoc = height + MIN_PLATFORM_HEIGHT;//Random.Range(height + MIN_PLATFORM_HEIGHT, height + MAX_PLATFORM_HEIGHT);

        Vector2 newPlatformLoc = new Vector2(newXLoc, newYLoc);

        // We don't want platforms to spawn close to the edges of the terrain
        if (newXLoc> (sideLength / 2 - 5f - PLATFORM_LENGTH) || newYLoc < (-sideLength / 2 + 5f)) { 
            return;
        }


        if (checkIfValidPlatformLocation(newPlatformLoc) ){
            platforms.Add(Instantiate(obj, newPlatformLoc, Quaternion.identity) as Transform);
        }
        if (count > 0) {
            addPlatformStack(obj, count - 1, newYLoc, newXLoc);
        }
    }

    /// <summary>
    /// Takes in the leftmost point of a potential platform position and 
    /// checks if a platform already exits that would overlap with it 
    /// </summary>
    /// <param name="position"></param>
    private bool checkIfValidPlatformLocation(Vector2 position) {
        int numPlatforms = platforms.Count();
        float yVal = position.y;
        float xVal = position.x;
        Vector2 curPlatform;
        for (int i = 0; i < numPlatforms; ++i) {
            curPlatform = platforms[i].position;
            float curXVal = curPlatform.x;
            float curYVal = curPlatform.y;

            if (Mathf.Abs(curXVal - xVal) < PLATFORM_LENGTH && Mathf.Abs(curYVal - yVal) < PLATFORM_HEIGHT) {
                return false;
            }
        }

        return true;
    }
    /// <summary>
    /// Checks if there is a platform that exists within minDistanceAway of the given point
    /// </summary>
    /// <param name="position"></param>
    /// <param name="minDistanceAway"></param>
    /// <returns></returns>
    private bool checkIfPlatformTooClose(Vector2 position, float minDistanceAway) {
        int numPlatforms = platforms.Count();
        float yVal = position.y;
        float xVal = position.x;
        Vector2 curPlatform;
        for (int i = 0; i < numPlatforms; ++i) {
            curPlatform = platforms[i].position;
            float curXVal = curPlatform.x;
            float curYVal = curPlatform.y;

            if (Mathf.Abs(curXVal - xVal) < minDistanceAway  && Mathf.Abs(curYVal - yVal) < minDistanceAway) {
                return false;
            }
        }

        return true;
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
                    enemies.Add(Instantiate(enemy, position, Quaternion.identity) as Transform);
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
    /// Possibly add goal items to the map depending on the given objective
    /// </summary>
    private void maybeAddGoalItems(Objective objective, int count)
    {
        //Only continue if the current objective specifies a goal collectible type
        if (objective.GoalCollectibleType.HasValue) {
            goalItemType = objective.GoalCollectibleType.Value;
            Transform goalItem = goalCollectibleList[goalItemType.Value];
            for (int i = 0; i < count; ++i) {
                Vector2 position;
                GetRandomPointAboveWalkable(MIN_ITEM_HEIGHT, out position);
                goalCollectibles.Add(Instantiate(goalItem, position, Quaternion.identity) as Transform);
            }
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
                if (!PersistentLevelSettings.settings.loadFromSave)
                    movePlayer (spaceshipPosition);
            }
        }
    }

    // Moves the player's starting position to be right next to the spaceship.
    private void movePlayer(Vector2 position) {
        GameObject.Find ("Player").transform.position = position;
    }

    /// <summary>
    /// Re-instantiate collectibles in the positions saved in the specified slots
    /// </summary>
    private void RestoreCollectibles(int slotId)
    {
        int numSavedCollectibles = PlayerPrefs.GetInt("numCollectibles" + slotId);
        for (int i = 0; i < numSavedCollectibles; ++i) {
            float xPos = PlayerPrefs.GetFloat("collectible" + i + "x" + slotId);
            float yPos = PlayerPrefs.GetFloat("collectible" + i + "y" + slotId);
            Vector2 pos = new Vector2(xPos, yPos);
            string collectibleType = PlayerPrefs.GetString("collectible" + i + "type" + slotId);
            //instantiate the correct collectible type
            for (int type = 0; type < collectibleList.Length; ++type) {
                if (collectibleType == collectibleList[type].gameObject.GetComponent<Collectible>().type)
                    collectibles.Add(Instantiate(collectibleList[type].transform, pos, Quaternion.identity) as Transform);
            }
        }
    }

    /// <summary>
    /// Re-instantiate special items in the positions saved in the specified slots
    /// </summary>
    private void RestoreSpecialItems(int slotId)
    {
        int numSavedSpecialItems = PlayerPrefs.GetInt("numSpecialItems" + slotId);
        for (int i = 0; i < numSavedSpecialItems; ++i) {
            float xPos = PlayerPrefs.GetFloat("specialItem" + i + "x" + slotId);
            float yPos = PlayerPrefs.GetFloat("specialItem" + i + "y" + slotId);
            Vector2 pos = new Vector2(xPos, yPos);
            specialItems.Add(Instantiate(specialItem, pos, Quaternion.identity) as Transform);
        }
    }

    /// <summary>
    /// Re-instantiate clouds in the positions saved in the specified slots
    /// </summary>
    private void RestoreClouds(int slotId)
    {
        int numSavedClouds = PlayerPrefs.GetInt("numClouds" + slotId);
        for (int i = 0; i < numSavedClouds; ++i) {
            float xPos = PlayerPrefs.GetFloat("cloud" + i + "x" + slotId);
            float yPos = PlayerPrefs.GetFloat("cloud" + i + "y" + slotId);
            Vector2 pos = new Vector2(xPos, yPos);
            string cloudType = PlayerPrefs.GetString("cloud" + i + "type" + slotId);
            //Instantiate the correct type of cloud
            Transform result = null;
            if (cloudType == poisonCloud.gameObject.tag)
                result = poisonCloud;
            else if (cloudType == slowCloud.gameObject.tag)
                result = slowCloud;
            clouds.Add(Instantiate(result, pos, Quaternion.identity) as Transform);
        }
    }

    /// <summary>
    /// Re-instantiate platforms in the positions saved in the specified slots
    /// </summary>
    private void RestorePlatforms(int slotId)
    {
        int numSavedPlatforms = PlayerPrefs.GetInt("numPlatforms" + slotId);
        for (int i = 0; i < numSavedPlatforms; ++i) {
            float xPos = PlayerPrefs.GetFloat("platform" + i + "x" + slotId);
            float yPos = PlayerPrefs.GetFloat("platform" + i + "y" + slotId);
            Vector2 pos = new Vector2(xPos, yPos);
            platforms.Add(Instantiate(jumpPlatform, pos, Quaternion.identity) as Transform);
        }
    }

    /// <summary>
    /// Re-instantiate enemies in the positions saved in the specified slots
    /// </summary>
    private void RestoreEnemies(int slotId)
    {
        int numSavedEnemies = PlayerPrefs.GetInt("numEnemies" + slotId);
        for (int i = 0; i < numSavedEnemies; ++i) {
            float xPos = PlayerPrefs.GetFloat("enemy" + i + "x" + slotId);
            float yPos = PlayerPrefs.GetFloat("enemy" + i + "y" + slotId);
            Vector2 pos = new Vector2(xPos, yPos + LOAD_GAME_HEIGHT_OFFSET);
            string enemyType = PlayerPrefs.GetString("enemy" + i + "type" + slotId);
            //Instantiate the correct type of enemy
            for (int type = 0; type < enemyList.Length; ++type) {
                if (enemyType == enemyList[type].gameObject.GetComponent<Enemy>().type)
                    enemies.Add(Instantiate(enemyList[type].transform, pos, Quaternion.identity) as Transform);
            }
        }
    }

    /// <summary>
    /// Re-instantiate obstacles in the positions saved in the specified slots
    /// </summary>
    private void RestoreObstacles(int slotId)
    {
        int numSavedObstacles = PlayerPrefs.GetInt("numObstacles" + slotId);
        for (int i = 0; i < numSavedObstacles; ++i) {
            float xPos = PlayerPrefs.GetFloat("obstacle" + i + "x" + slotId);
            float yPos = PlayerPrefs.GetFloat("obstacle" + i + "y" + slotId);
            Vector2 pos = new Vector2(xPos, yPos + LOAD_GAME_HEIGHT_OFFSET);
            string obstacleType = PlayerPrefs.GetString("obstacle" + i + "type" + slotId);
            //Instantiate the correct type of cloud
            Transform result = null;
            if (obstacleType == bouncyBox.gameObject.tag)
                result = bouncyBox;
            else if (obstacleType == stickyBox.gameObject.tag)
                result = stickyBox;
            obstacles.Add(Instantiate(result, pos, Quaternion.identity) as Transform);
        }
    }

    /// <summary>
    /// Re-instantiate goal items in the positions saved in the specified slots
    /// </summary>
    private void RestoreGoalItems(int slotId)
    {
        int numSavedGoalItems = PlayerPrefs.GetInt("numGoalItems" + slotId);
        int savedGoalItemType = PlayerPrefs.GetInt("goalItemType" + slotId);
        //check whether the saved goal item type is valid.
        //if it isn't, then the saved game wasn't using goal items
        if (savedGoalItemType == -1) {
            goalItemType = null;
        } else {
            goalItemType = savedGoalItemType;
            for (int i = 0; i < numSavedGoalItems; ++i) {
                float xPos = PlayerPrefs.GetFloat("goalItem" + i + "x" + slotId);
                float yPos = PlayerPrefs.GetFloat("goalItem" + i + "y" + slotId);
                Vector2 pos = new Vector2(xPos, yPos);
                goalCollectibles.Add(Instantiate(goalCollectibleList[goalItemType.Value], pos, Quaternion.identity) as Transform);
            }
        }
    }

    public void RemoveFromScene(NonPlayerObject npo) {
        if (collectibles.Contains (npo.transform))
            collectibles.Remove (npo.transform);
        if (specialItems.Contains(npo.transform))
            specialItems.Remove(npo.transform);
        if (goalCollectibles.Contains(npo.transform))
            goalCollectibles.Remove(npo.transform);
    }

    public int GetSpecialItemsRemaining()
    {
        return specialItems.Count;
    }

    public int GetGoalItemsRemaining()
    {
        return goalCollectibles.Count;
    }

    /// <summary>
    /// Save the position and type of all items currently on the map
    /// </summary>
    public void SaveItems(int slotId)
    {
        //Save collectibles
        PlayerPrefs.SetInt("numCollectibles" + slotId, collectibles.Count);
        for (int i = 0; i < collectibles.Count; ++i) {
            PlayerPrefs.SetFloat("collectible" + i + "x" + slotId, collectibles[i].position.x);
            PlayerPrefs.SetFloat("collectible" + i + "y" + slotId, collectibles[i].position.y);
            PlayerPrefs.SetString("collectible" + i + "type" + slotId, collectibles[i].gameObject.GetComponent<Collectible>().type);
        }
        //Save special items
        PlayerPrefs.SetInt("numSpecialItems" + slotId, specialItems.Count);
        for (int i = 0; i < specialItems.Count; ++i) {
            PlayerPrefs.SetFloat("specialItem" + i + "x" + slotId, specialItems[i].position.x);
            PlayerPrefs.SetFloat("specialItem" + i + "y" + slotId, specialItems[i].position.y);
        }
        //Save clouds (insert obligatory FFVII joke here)
        PlayerPrefs.SetInt("numClouds" + slotId, clouds.Count);
        for (int i = 0; i < clouds.Count; ++i) {
            PlayerPrefs.SetFloat("cloud" + i + "x" + slotId, clouds[i].position.x);
            PlayerPrefs.SetFloat("cloud" + i + "y" + slotId, clouds[i].position.y);
            PlayerPrefs.SetString("cloud" + i + "type" + slotId, clouds[i].gameObject.tag);
        }
        //Save platforms
        PlayerPrefs.SetInt("numPlatforms" + slotId, platforms.Count);
        for (int i = 0; i < platforms.Count; ++i) {
            PlayerPrefs.SetFloat("platform" + i + "x" + slotId, platforms[i].position.x);
            PlayerPrefs.SetFloat("platform" + i + "y" + slotId, platforms[i].position.y);
        }
        //Save enemies
        PlayerPrefs.SetInt("numEnemies" + slotId, enemies.Count);
        for (int i = 0; i < enemies.Count; ++i) {
            PlayerPrefs.SetFloat("enemy" + i + "x" + slotId, enemies[i].position.x);
            PlayerPrefs.SetFloat("enemy" + i + "y" + slotId, enemies[i].position.y);
            PlayerPrefs.SetString("enemy" + i + "type" + slotId, enemies[i].gameObject.GetComponent<Enemy>().type);
        }
        //Save obstacles
        PlayerPrefs.SetInt("numObstacles" + slotId, obstacles.Count);
        for (int i = 0; i < obstacles.Count; ++i) {
            PlayerPrefs.SetFloat("obstacle" + i + "x" + slotId, obstacles[i].position.x);
            PlayerPrefs.SetFloat("obstacle" + i + "y" + slotId, obstacles[i].position.y);
            PlayerPrefs.SetString("obstacle" + i + "type" + slotId, obstacles[i].gameObject.tag);
        }
        //Save goal collectibles
        PlayerPrefs.SetInt("numGoalItems" + slotId, goalCollectibles.Count);
        PlayerPrefs.SetInt("goalItemType" + slotId, goalItemType.HasValue ? goalItemType.Value : -1);
        for (int i = 0; i < goalCollectibles.Count; ++i) {
            PlayerPrefs.SetFloat("goalItem" + i + "x" + slotId, goalCollectibles[i].position.x);
            PlayerPrefs.SetFloat("goalItem" + i + "y" + slotId, goalCollectibles[i].position.y);
        }
    }
}
