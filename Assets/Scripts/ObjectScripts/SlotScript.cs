using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class SlotScript : MonoBehaviour {

    private InventoryScript inventory;
    public Collectible item;
   
	// Use this for initialization
	void Start () {
        inventory = GameObject.Find("Inventory").GetComponent<InventoryScript>();
	}
	
    public void OnClick() {
        // we need to trust that whoever made the slot is remembering to update the name/type of thing
        // this slot is holding.
        print ("OnClick called");
        Assert.IsNotNull(item);
        inventory.RemoveItemFromInventory (item);
    }
}
