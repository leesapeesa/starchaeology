using UnityEngine;
using System.Collections;
using System;

public class PointCollectible : Collectible
{
    public override void OnCollect(InventoryScript inventory)
    {
        print("LOLOLOLOLOL");
        PersistentPlayerSettings.settings.levelScore += value;
        //PersistentPlayerSettings.settings.scoreText.text = "Score: " + PersistentPlayerSettings.settings.score.ToString();
        print("Score: " + PersistentPlayerSettings.settings.levelScore);
    }

    public override void OnUse(PlayerCharacter2D player)
    {
        throw new NotImplementedException();
    }
}
