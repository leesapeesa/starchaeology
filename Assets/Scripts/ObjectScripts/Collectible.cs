using UnityEngine;
using System.Collections;

public enum ItemFunction
{
    Score,
    Health,
    TimeItem,
    Weapon
}

public class Collectible : NonPlayerObject {
    public string type;
    public Sprite itemIcon;
    public int value;

    public ItemFunction function;
    void Start() {
        itemIcon = gameObject.GetComponent<SpriteRenderer> ().sprite;
    }

    public void CollectedItem() {
        PersistentPlayerSettings.settings.CollectedItem (this);
//        if (GameObject.Find ("ObjectManager") == null) {
//            // ObjectManager might be removed first.
//            return;
//        }
//        ItemManager itemManager = GameObject.Find ("ObjectManager").GetComponent<ItemManager> ();
//        itemManager.AddToInventory (this);
    }
}
