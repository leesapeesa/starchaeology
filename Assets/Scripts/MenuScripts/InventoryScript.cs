using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InventoryScript : MonoBehaviour {

    private ItemManager itemManager;
    private GameObject inventoryPanel;
    public float inX, inY = 0f;
    public int slotsX = 4;
    public int slotsY = 5;
    private List<Collectible> slots = new List<Collectible> ();
    // Use this for initialization
    void Start () {
        itemManager = GameObject.Find ("ObjectManager").GetComponent<ItemManager> ();
        for (int i = 0; i < slotsX * slotsY; ++i) {
            slots.Add (new Collectible());
        }
    }
}
