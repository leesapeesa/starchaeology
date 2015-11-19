using UnityEngine;
using System.Collections;
using System;

public class TimeCollectible : Collectible
{
    public const string typeString = "Time";

    public override string type { get { return typeString; } }

    public override void OnCollect()
    {
        InventoryScript.inventory.AddItemToInventory(this);
    }

    public override void OnUse(PlayerCharacter2D player)
    {
        player.AddTime (value);
    }

    public override void AddToPossibleInventory() {
        InventoryScript.inventory.AddItemToPossibleInventory(this);
    }

}
