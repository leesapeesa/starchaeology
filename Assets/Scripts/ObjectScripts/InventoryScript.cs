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
    public int totalSlots = 10;
    public GameObject slot;
    public GameObject healthCollectible;
    public GameObject timerCollectible;
    public GameObject specialItemCollectible;

    private Dictionary<string, Entry> inventorySlots;
    private GameObject inventoryGUI;
    private GameObject slotPanel;
    private PlayerCharacter2D player;
    private AudioSource audioSource;

    void Awake () {
        // We also want a persistent inventory.
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
    }

    void OnDestroy() {
        
    }

    // Use this for initialization
    void Start () {
        slotPanel = GameObject.Find ("SlotPanel");
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Adds item to the inventory and increments the amount
    /// contained in the inventory.
    /// </summary>
    /// <param name="item">Item.</param>
    public void AddItemToInventory(Collectible item) {
        if (!inventorySlots.ContainsKey(item.type))
            inventorySlots.Add(item.type, new Entry(item, 1));
        else
            inventorySlots[item.type]++;

        DrawInventory();
    }

    public void AddItemToPossibleInventory(Collectible item) {
        if (!inventorySlots.ContainsKey(item.type))
            inventorySlots.Add(item.type, new Entry(item, 0));

        DrawInventory();
    }


    /// <summary>
    /// Remove item from inventory by decrementing the amount
    /// currently stored in the inventory.
    /// </summary>
    /// <param name="item">Item.</param>
    public void RemoveItemFromInventory(Collectible item) {
        Assert.IsNotNull (item);
        if (!inventorySlots.ContainsKey (item.type)) {
            print ("Item missing");
            return;
        }

        // Only remove items from the inventory if the count is 0
        int numItem = inventorySlots[item.type].amount;
        if (numItem == 0) {
            return;
        }

        // Update GameObject Counter;
        player = GameObject.Find ("Player").transform.GetComponent<PlayerCharacter2D> ();
        item.OnUse(player);
        --inventorySlots [item.type];

        DrawInventory();
        audioSource.Play();
    }

    public void RemoveItemFromPossibleInventory(Collectible item) {

        Assert.IsNotNull(item);
        if (!inventorySlots.ContainsKey(item.type)) {
            return;
        }

        // Update GameObject Counter;
        player = GameObject.Find("Player").transform.GetComponent<PlayerCharacter2D>();
        item.OnUse(player);
        --inventorySlots[item.type];

        inventorySlots.Remove(item.type);

        DrawInventory();
        audioSource.Play();
    }

    /// <summary>
    /// Draws the GUI for the inventory.
    /// </summary>
    public void DrawInventory() {
        RemoveAllOldSlots();
        foreach (KeyValuePair<string, Entry> pair in inventorySlots) {
            Entry entry = pair.Value;

            Collectible collectible = entry.item;

            // We need to make the slot.
            GameObject go = Instantiate(slot, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;

            // If we don't have any of the item, grey it out
            if (entry.amount == 0) {
                go.transform.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                go.transform.FindChild("Item").gameObject.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            } else {
                go.transform.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
                go.transform.FindChild("Item").gameObject.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }

            go.transform.GetComponent<SlotScript>().item = collectible;
            go.transform.FindChild("Item").gameObject.GetComponent<Image>().sprite = collectible.itemIcon;
            go.transform.FindChild("Counter").gameObject.GetComponent<Text>().text = entry.amount.ToString();
            go.transform.FindChild("Key").gameObject.GetComponent<Text>().text = entry.item.buttonName;
            go.transform.SetParent(slotPanel.transform);
            go.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    /// <summary>
    /// Checks whether or not the inventory contains one or more items of the specified type
    /// </summary>
    public bool HasItemOfType(string type)
    {
        return inventorySlots.ContainsKey(type);
    }

    /// <summary>
    /// Retrieve an item of the specified type from the inventory.
    /// </summary>
    public Collectible GetItemOfType(string type)
    {
        Assert.IsTrue(HasItemOfType(type));
        Entry typeEntry = inventorySlots[type];
        return typeEntry.item;
    }

    /// <summary>
    /// Removes the slots that were previously drawn.
    /// </summary>
    void RemoveAllOldSlots() {
        slotPanel = GameObject.Find ("SlotPanel");
        foreach (Transform child in slotPanel.transform) {
            Destroy(child.gameObject);
        }
    }

    public void EmptyInventory() {
        inventorySlots.Clear ();
    }

    /// <summary>
    /// Save the inventory to permanent storage at the specified slot
    /// </summary>
    public void SaveInventory(int slotId)
    {
        //First, keep track of how many types of items we have in the inventory
        PlayerPrefs.SetInt("numSlots" + slotId, inventorySlots.Count);
        //Then, loop through and save all items to persistent storage
        int itemIndex = 0;
        foreach (KeyValuePair<string, Entry> p in inventorySlots) {
            int count = p.Value.amount;
            PlayerPrefs.SetString("inventorySlot" + itemIndex + "type" + slotId, p.Key);
            PlayerPrefs.SetInt("inventorySlot" + itemIndex + "count" + slotId, count);
            ++itemIndex;
        }
    }

    /// <summary>
    /// Restore an inventory from a saved game
    /// </summary>
    public void LoadInventory(int slotId)
    {
        //First, clear out anything currently in the inventory.
        EmptyInventory();

        //Retrieve every saved inventory slot
        int numSlots = PlayerPrefs.GetInt("numSlots" + slotId);
        for (int i = 0; i < numSlots; ++i) {
            string itemType = PlayerPrefs.GetString("inventorySlot" + i + "type" + slotId);
            int itemCount = PlayerPrefs.GetInt("inventorySlot" + i + "count" + slotId);
            //Build a new inventory item based on the save type
            Collectible item = null;
            switch (itemType) {
                //This is rather ugly, but unfortunately Collectible is a MonoBehaviour, and Unity does not allow us
                //to create new MonoBehaviours with the "new" keyword - you are only allowed to create a GameObject
                //with the desired MonoBehaviour attached. To get around this, we assign the Collectible prefabs
                //to this script in the inspector, and retrieve the desired Collectible from the prefabs.
                case HealthCollectible.typeString:
                    item = healthCollectible.GetComponent<HealthCollectible>();
                    break;
                case TimeCollectible.typeString:
                    item = timerCollectible.GetComponent<TimeCollectible>();
                    break;
                case SpecialItemCollectible.typeString:
                    item = specialItemCollectible.GetComponent<SpecialItemCollectible>();
                    break;
                default:
                    break;
            }

            //Build a corresponding entry for the reconstructed inventory item
            Entry entry = new Entry(item, itemCount);
            //Finally, add the entry to the inventory
            inventorySlots.Add(itemType, entry);
        }
    }

    /// <summary>
    /// If any health item exists in the inventory, use it
    /// </summary>
    public void MaybeUseHealthItem()
    {
        MaybeUseItem(healthCollectible.GetComponent<HealthCollectible>());
    }

    /// <summary>
    /// If any timer item exists in the inventory, use it
    /// </summary>
    public void MaybeUseTimeItem()
    {
        MaybeUseItem(timerCollectible.GetComponent<TimeCollectible>());
    }

    /// <summary>
    /// Try to use the specified item, if we currently have it in the inventory
    /// </summary>
    private void MaybeUseItem(Collectible item)
    {
        if (!inventorySlots.ContainsKey(item.type) || inventorySlots[item.type].amount == 0) {
            return;
        }

        RemoveItemFromInventory(item);
    }
}
                                                                                                           