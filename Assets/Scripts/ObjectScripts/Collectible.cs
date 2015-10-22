using UnityEngine;
using System.Collections;


public abstract class Collectible : NonPlayerObject {
    public string type;
    public Sprite itemIcon;
    public int value;

    void Start() {
        itemIcon = gameObject.GetComponent<SpriteRenderer> ().sprite;
    }

    public abstract void OnUse(PlayerCharacter2D player);
    public abstract void OnCollect(InventoryScript inventory);
}
