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

    private Dictionary<Collectible, int> inventorySlots;
    private GameObject inventoryGUI;
    private GameObject slotPanel;
    private PlayerCharacter2D player;
    private AudioSource audioSource;

    void Awake () {
        if (inventory == null) {
            SetUpInventory();
            DontDestroyOnLoad (gameObject);
            inventory = this;
        } else if (inventory != this) {
            Destroy (gameObject);
        }
    }

    void SetUpInventory() {
        inventorySlots = new Dictionary<Collectible, int>();
        print ("setting up inventory");
    }

    void OnDestroy() {
        print ("Destroyed Inventory");
    }

    // Use this for initialization
    void Start () {
        slotPanel = GameObject.Find ("SlotPanel");
        if (GameObject.Find ("Player") != null)
            player = GameObject.Find ("Player").transform.GetComponent<PlayerCharacter2D> ();
        audioSource = GetComponent<AudioSource>();
    }

    public void AddItemToInventory(Collectible item) {
        print ("added Item to inventory!");
        if (!inventorySlots.ContainsKey (item))
            inventorySlots.Add(item, 1);
        else
            ++inventorySlots [item];
    }

    public void RemoveItemFromInventory(Collectible item) {
        Assert.IsNotNull (item);
        if (!inventorySlots.ContainsKey (item)) {
            print ("Item missing");
            return;
        }
        // Update GameObject Counter;
        item.OnUse(player);
        --inventorySlots [item];
        if (inventorySlots [item] == 0) {
            inventorySlots.Remove(item);
        }
        audioSource.Play();
    }

    public void DrawInventory() {
        print ("Drawing Inventory");
        RemoveAllOldSlots ();
        foreach (KeyValuePair<Collectible, int> pair in inventorySlots) {
            print ("pair" + pair.Key.ToString());
            Collectible item = pair.Key;
            // We need to make the slot.
            GameObject go = Instantiate (slot, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
            go.transform.GetComponent<SlotScript>().item = item;
            go.transform.FindChild ("Item").gameObject.GetComponent<Image> ().sprite = item.itemIcon;
            go.transform.FindChild("Counter").gameObject.GetComponent<Text>().text = pair.Value.ToString();
            go.transform.SetParent (slotPanel.transform);
            go.transform.localScale = new Vector3 (1, 1, 1);
        }
    }

    void RemoveAllOldSlots() {
        slotPanel = GameObject.Find ("SlotPanel");
        foreach (Transform child in slotPanel.transform) {
            Destroy(child.gameObject);
        }
    }
    public void EmptyInventory() {
        inventorySlots.Clear ();
    }
}
                                                                                                           