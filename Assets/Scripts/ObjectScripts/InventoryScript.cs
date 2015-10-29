using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System;

public class InventoryScript : MonoBehaviour {

    // We can't use Pair or Tuple in Unity and we need it for our
    // inventory to store a copy of collectible and the amount
    // since each collectible is a unique object.
    // Potentially make this into its own class.
    private struct Entry
    {
        public Collectible item;
        public int amount;

        public Entry (Collectible item, int amount) {
            this.item = item;
            this.amount = amount;
        }

        public static Entry operator ++(Entry e) {
            e.amount++;
            return e;
        }

        public static Entry operator --(Entry e) {
            e.amount--;
            return e;
        }

        public bool Empty() {
            return amount == 0;
        }
    }

    public static InventoryScript inventory;

    public float inX, inY = 0f;
    public int totalSlots = 20;
    public GameObject slot;

    private Dictionary<string, Entry> inventorySlots;
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
        inventorySlots = new Dictionary<string, Entry>();
        print ("setting up inventory");
    }

    void OnDestroy() {
        print ("Destroyed Inventory");
    }

    // Use this for initialization
    void Start () {
        slotPanel = GameObject.Find ("SlotPanel");
        audioSource = GetComponent<AudioSource>();
    }

    public void AddItemToInventory(Collectible item) {
        print ("added Item to inventory!");
        if (!inventorySlots.ContainsKey (item.type))
            inventorySlots.Add(item.type, new Entry(item, 1));
        else
            inventorySlots [item.type]++;
    }

    public void RemoveItemFromInventory(Collectible item) {
        Assert.IsNotNull (item);
        if (!inventorySlots.ContainsKey (item.type)) {
            print ("Item missing");
            return;
        }
        // Update GameObject Counter;
        player = GameObject.Find ("Player").transform.GetComponent<PlayerCharacter2D> ();
        item.OnUse(player);
        --inventorySlots [item.type];
        if (inventorySlots [item.type].Empty()) {
            inventorySlots.Remove(item.type);
        }
        DrawInventory();
        audioSource.Play();
    }

    public void DrawInventory() {
        print ("Drawing Inventory");
        RemoveAllOldSlots ();
        foreach (KeyValuePair<string, Entry> pair in inventorySlots) {
            print ("pair" + pair.Key.ToString());
            Entry entry = pair.Value;
            Collectible collectible = entry.item;
            // We need to make the slot.
            GameObject go = Instantiate (slot, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
            go.transform.GetComponent<SlotScript>().item = collectible;
            go.transform.FindChild ("Item").gameObject.GetComponent<Image> ().sprite = collectible.itemIcon;
            go.transform.FindChild("Counter").gameObject.GetComponent<Text>().text = entry.amount.ToString();
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
                                                                                                           