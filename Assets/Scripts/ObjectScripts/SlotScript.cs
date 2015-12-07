using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class SlotScript : MonoBehaviour {

    public Collectible item;
	
    /// <summary>
    /// If slot is clicked, then one of the items that was contained in the inventory will
    /// be deleted.
    /// </summary>
    public void OnClick() {
        // we need to trust that whoever made the slot is remembering to update the name/type of thing
        // this slot is holding.
        Assert.IsNotNull(item);
        if (item.usable)
            InventoryScript.inventory.RemoveItemFromInventory (item);
    }
}
