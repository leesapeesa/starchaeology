using UnityEngine;
using System.Collections;
using System;

public class TimeCollectible : Collectible
{
    public override void OnCollect()
    {
        InventoryScript.inventory.AddItemToInventory(this);
    }

    public override void OnUse(PlayerCharacter2D player)
    {
        player.AddTime (value);
    }
}
