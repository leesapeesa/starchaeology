using UnityEngine;
using System.Collections;
using System;

public class PointCollectible : Collectible
{
    public const string typeString = "Point";

    public static int points = 1;

    public override bool usable { get { return false; } }

    public override string type { get { return typeString; } }

    protected override void Start()
    {
        base.Start();
        value = points;
    }

    public override void OnCollect()
    {
        PersistentPlayerSettings.settings.levelScore += value;
        PersistentPlayerSettings.settings.overallScore += value;
    }

    public override void OnUse(PlayerCharacter2D player)
    {
        throw new NotImplementedException();
    }

    public override void AddToPossibleInventory() {
        //Nothing to do here; coins don't go in inventory
    }
}
