using UnityEngine;
using System.Collections;


public abstract class Collectible : NonPlayerObject {
    public string type;
    public Sprite itemIcon;
    public int value;
    public AudioClip pickupSound;

    protected virtual void Start() {
        // Icon Image for inventory.
        itemIcon = gameObject.GetComponent<SpriteRenderer> ().sprite;
    }

    public virtual bool usable { get { return true; } }

    public abstract void OnUse(PlayerCharacter2D player);
    public abstract void OnCollect();
}
