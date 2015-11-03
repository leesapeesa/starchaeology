using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour {

    public int boxCount = 5;
    public int collectCount = 10;
    public int slowCloudCount = 5;
    public int poisonCloudCount = 4;
    public int enemyCount = 3;

    public bool allCollected = false;

    public Transform bouncyBox;
    public Transform stickyBox;
    public Transform slowCloud;
    public Transform poisonCloud;
    public Transform[] enemyList;
    public Transform[] collectibleList;

    private float sideLength = 25f;
    private float closestToEdge = 5f;
    private Vector2[] heights;
    private List<Transform> collectibles;
    private float gravityEffect;
    private Dictionary<Transform, float> pTable; //enemy type probability table

    // Use this for initialization
    void Start() {
        heights = GameObject.FindObjectOfType<TerrainCreator> ().getPathHeights();
        sideLength = PersistentTerrainSettings.settings.sideLength - closestToEdge;
        print ("sideLength " + PersistentTerrainSettings.settings.sideLength);
        gravityEffect = PersistentTerrainSettings.settings.gravityEffect;
        enemyCount = PersistentTerrainSettings.settings.numEnemies;

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
        addObjects(slowCloud, slowCloudCount, 1);
        addObjects(stickyBox, boxCount, 5);
        addObjects(poisonCloud, poisonCloudCount);
        addEnemies(enemyCount);
    }

    void Update() {
        if (!collectibles.Any ()) {
            allCollected = true;
        }
    }

    private void addCollectibles() {
        heights = GameObject.FindObjectOfType<TerrainCreator> ().getPathHeights();
        for (int i = 0; i < collectCount; ++i) {
            float xCoor = Random.Range(0, sideLength);
            float height = heights[(int)xCoor].y + Random.Range(3, PersistentTerrainSettings.settings.height / 1.5f);
            Vector3 position = new Vector3(xCoor - sideLength / 2, height);
            Transform collect = collectibleList[Random.Range(0, collectibleList.Length)].transform;
            collectibles.Add (Instantiate(collect, position, Quaternion.identity) as Transform);
        }
    }

    private void addObjects(Transform obj, int count, float y = 1f)
    {
        for (int i = 0; i < count; ++i) {
            Vector3 position = new Vector3(Random.Range(-sideLength / 2, sideLength / 2), y);
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
                    Vector3 position = new Vector3(Random.Range(-sideLength / 2, sideLength / 2), 5f);
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
