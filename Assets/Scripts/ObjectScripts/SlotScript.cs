using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class SlotScript : MonoBehaviour {

    public Collectible item;
	
    public void OnClick() {
        // we need to trust that whoever made the slot is remembering to update the name/type of thing
        // this slot is holding.
        print ("OnClick called");
        Assert.IsNotNull(item);
        InventoryScript.inventory.RemoveItemFromInventory (item);
    }
}
