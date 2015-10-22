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
    public Transform groundPathEnemy;
    public Transform[] collectibleList;

    private float sideLength = 25f;
    private float closestToEdge = 5f;
    private float[] heights;
    private List<Transform> collectibles;
    private float gravityEffect;
    // private InventoryScript inventory;

    // Use this for initialization
    void Start() {
        //inventory = GameObject.Find ("Inventory").GetComponent<InventoryScript> ();
        heights = GameObject.FindObjectOfType<TerrainCreator> ().GetHeights();
        sideLength = PersistentTerrainSettings.settings.sideLength - closestToEdge;
        gravityEffect = PersistentTerrainSettings.settings.gravityEffect;

        GameObject newStickyBox = stickyBox.gameObject;
        Rigidbody2D stickyBoxRigidBody = newStickyBox.GetComponent<Rigidbody2D>();
        stickyBoxRigidBody.gravityScale = gravityEffect;

        GameObject newBouncyBox = bouncyBox.gameObject;
        Rigidbody2D bouncyBoxRigidBody = newBouncyBox.GetComponent<Rigidbody2D>();
        bouncyBoxRigidBody.gravityScale = gravityEffect;

        GameObject newGroundPathEnemy = groundPathEnemy.gameObject;
        Rigidbody2D groundPathEnemyRigidBody = newGroundPathEnemy.GetComponent<Rigidbody2D>();
        groundPathEnemyRigidBody.gravityScale = gravityEffect;

        collectibles = new List<Transform> ();
        addCollectibles ();
        addObjects(bouncyBox, boxCount, 2);
        addObjects(slowCloud, slowCloudCount, 1);
        addObjects(stickyBox, boxCount, 5);
        addObjects(poisonCloud, poisonCloudCount);
        addObjects(groundPathEnemy, enemyCount, 5);
    }

    void Update() {
        if (!collectibles.Any ()) {
            allCollected = true;
        }
    }

    private void addCollectibles() {
        for (int i = 0; i < collectCount; ++i) {
            float xCoor = Random.Range(0, sideLength);
            float height = heights[(int)xCoor] + Random.Range (0, 3);
            height = Mathf.Max(height, 5);
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

    public void RemoveFromScene(NonPlayerObject npo) {
        if (collectibles.Contains (npo.transform))
            collectibles.Remove (npo.transform);
    }

    //public void AddToInventory(Collectible item) {
    //    inventory.AddItemToInventory (item);
    //}
}
