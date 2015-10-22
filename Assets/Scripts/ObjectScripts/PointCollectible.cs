using UnityEngine;
using System.Collections;
using System;

public class PointCollectible : Collectible
{
    public override void OnCollect(InventoryScript inventory)
    {
        print("LOLOLOLOLOL");
        PersistentPlayerSettings.settings.score += value;
        //PersistentPlayerSettings.settings.scoreText.text = "Score: " + PersistentPlayerSettings.settings.score.ToString();
        print("Score: " + PersistentPlayerSettings.settings.score);
    }

    public override void OnUse(PlayerCharacter2D player)
    {
        throw new NotImplementedException();
    }
}
