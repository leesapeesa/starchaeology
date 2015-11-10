using UnityEngine;
using System.Collections;
using System;

public class PointCollectible : Collectible
{
    public static int points = 2;

    public override bool usable { get { return false; } }

    protected override void Start()
    {
        base.Start();
        value = points;
    }

    public override void OnCollect()
    {
        PersistentPlayerSettings.settings.levelScore += value;
        //PersistentPlayerSettings.settings.scoreText.text = "Score: " + PersistentPlayerSettings.settings.score.ToString();
        print("Score: " + PersistentPlayerSettings.settings.levelScore);
    }

    public override void OnUse(PlayerCharacter2D player)
    {
        throw new NotImplementedException();
    }
}
