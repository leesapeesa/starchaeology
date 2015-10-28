using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System;

public class InventoryScript : MonoBehaviour {

    public static InventoryScript inventory;

    public float inX, inY = 0f;
    public int totalSlots = 20;
    public GameObject slot;

    private Dictionary<string, GameObject> inventorySlots;
    private GameObject slotPanel;
    private PlayerCharacter2D player;

    void Awake () {
        if (inventory == null) {
            DontDestroyOnLoad (gameObject);
            inventory = this;
        } else if (inventory != this) {
            Destroy (gameObject);
        }
    }

    // Use this for initialization
    void Start () {
        slotPanel = GameObject.Find ("SlotPanel");
        print (slotPanel.ToString ());
        inventorySlots = new Dictionary<string, GameObject>();
        player = GameObject.Find ("Player").transform.GetComponent<PlayerCharacter2D> ();
    }
    void OnDestroy() {
        print ("Destroy Inventory");
        // For now, every time we load a new level, we'll clear our inventory.
        inventorySlots.Clear ();
    }

    public void AddItemToInventory(Collectible item) {
        print ("added Item to inventory!");
        if (!inventorySlots.ContainsKey (item.type)) {
            // We need to make the slot.
            GameObject go = Instantiate (slot, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
            go.transform.GetComponent<SlotScript>().item = item;
            go.transform.FindChild ("Item").gameObject.GetComponent<Image> ().sprite = item.itemIcon;
            go.transform.SetParent (slotPanel.transform);
            go.transform.localScale = new Vector3 (1, 1, 1);
            inventorySlots.Add (item.type, go);
            print ("New Item");
        }
        // Update GameObject Counter;
        ChangeInventory(item.type, 1);
    }

    public void RemoveItemFromInventory(Collectible item) {
        Assert.IsNotNull (item);
        if (!inventorySlots.ContainsKey (item.type)) {
            print ("Item missing");
            return;
        }
        // Update GameObject Counter;
        item.OnUse(player);
        ChangeInventory(item, -1);
    }

    void ChangeInventory(Collectible item, int amount) {
        ChangeInventory (item.type, amount);
    }

    void ChangeInventory(string type, int amount) {
        GameObject go = inventorySlots [type];
        int counter = Int32.Parse(go.transform.FindChild ("Counter").gameObject.GetComponent<Text> ().text);
        counter += amount;
        if (counter == 0) {
            print ("No more item: " + type);
            inventorySlots.Remove (type);
            GameObject.Destroy (go);
        } else {
            go.transform.FindChild ("Counter").gameObject.GetComponent<Text> ().text = counter.ToString();
        }
    }
}
                                                                                                           