using UnityEngine;
using System.Collections;


public abstract class Collectible : NonPlayerObject {
    public string type;
    public Sprite itemIcon;
    public int value;
    public AudioClip pickupSound;

    void Start() {
        // Icon Image for inventory.
        itemIcon = gameObject.GetComponent<SpriteRenderer> ().sprite;
    }

    public abstract void OnUse(PlayerCharacter2D player);
    public abstract void OnCollect();
}
