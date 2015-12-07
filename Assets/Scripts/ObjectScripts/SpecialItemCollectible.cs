using UnityEngine;
using System.Collections;

public class SpecialItemCollectible : Collectible {

    public const string typeString = "SpecialItem";

    public override bool usable { get { return false; } }

    public override string type { get { return typeString; } }

    public override void OnCollect()
    {
        PersistentPlayerSettings.settings.levelScore += value;
        PersistentPlayerSettings.settings.overallScore += value;
        InventoryScript.inventory.AddItemToInventory(this);
    }

    public override void OnUse(PlayerCharacter2D player)
    {
        return; //the special item should not do anything.
    }

    public override void AddToPossibleInventory() {
    }
}
