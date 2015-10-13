using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class InventoryScript : MonoBehaviour {

    private GameObject inventoryPanel;
    public float inX, inY = 0f;
    public int slotsX = 4;
    public int slotsY = 5;
    public Sprite testing;
    private GameObject[] inventorySlots;
    private List<Collectible> collected;
    // Use this for initialization
    void Start () {
        inventorySlots = GameObject.FindGameObjectsWithTag ("ItemSlot");
        collected = new List<Collectible> ();
        // print ("slot size: " + inventorySlots.Length);
        // inventorySlots [19].transform.Find("Item").gameObject.GetComponent<Image>().sprite = testing;
        // print (testing);
    }

    public void AddItemToInventory(Collectible item) {
        print ("added Item to inventory!");
        collected.Add (item);
        AssignIcons ();
    }
    
    void SetSlotToItemIcon(int slot, Collectible item) {
        Assert.IsTrue (slot >= 0 && slot < inventorySlots.Length);
        inventorySlots [slot].transform.Find ("Item").gameObject.GetComponent<Image> ().sprite = item.itemIcon;
    }

    void RemoveItemFromInventory(Collectible item) {
        collected.Remove (item);
        AssignIcons ();
    }

    void AssignIcons() {
        foreach (Collectible c in collected) {
            for (int i = inventorySlots.Length - 1; i >= inventorySlots.Length - collected.Count; --i) {
                SetSlotToItemIcon(i, c);
            }
        }
    }

}
                                                                                                           