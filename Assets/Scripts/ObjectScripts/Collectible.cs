using UnityEngine;
using System.Collections;

public class Collectible : NonPlayerObject {
    public string type;
    public Sprite itemIcon;

    void Start() {
        itemIcon = gameObject.GetComponent<SpriteRenderer> ().sprite;
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
