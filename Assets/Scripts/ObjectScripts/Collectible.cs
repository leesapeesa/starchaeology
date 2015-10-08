using UnityEngine;
using System.Collections;

public class Collectible : NonPlayerObject {
    public Texture image;
    public string type;
	
    public Collectible() {
        type = "empty";
    }

    public void CollectedItem() {
        if (GameObject.Find ("ObjectManager") == null) {
            // ObjectManager might be removed first.
            return;
        }
        ItemManager itemManager = GameObject.Find ("ObjectManager").GetComponent<ItemManager> ();
        itemManager.AddToInventory (this);
    }
}
