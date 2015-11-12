using UnityEngine;
using System.Collections;

public class SpecialItemCollectible : Collectible {

    public override bool usable { get { return false; } }

    public override void OnCollect()
    {
        PersistentPlayerSettings.settings.levelScore += value;
        InventoryScript.inventory.AddItemToInventory(this);
    }

    public override void OnUse(PlayerCharacter2D player)
    {
        Debug.Log("Player clicked on golden idol! What permanent thing should we do here?");
    }
}
