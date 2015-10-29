using UnityEngine;
using System.Collections;

public class InventorySceenScript : MonoBehaviour {

    private InventoryScript inventory;
	// Use this for initialization
	void Start () {
        inventory = InventoryScript.inventory;
	}
	
	public void DrawInventory() {
        inventory.DrawInventory ();
    }
}
