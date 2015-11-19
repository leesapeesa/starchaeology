using UnityEngine;
using System.Collections;


public abstract class Collectible : NonPlayerObject {
    public Sprite itemIcon;
    public int value;
    public AudioClip pickupSound;

    protected virtual void Start() {
        // Icon Image for inventory.
        itemIcon = gameObject.GetComponent<SpriteRenderer> ().sprite;
        AddToPossibleInventory();
    }

    public virtual bool usable { get { return true; } }

    public abstract string type { get; }

    public abstract void OnUse(PlayerCharacter2D player);
    public abstract void OnCollect();
    public abstract void AddToPossibleInventory();
}
