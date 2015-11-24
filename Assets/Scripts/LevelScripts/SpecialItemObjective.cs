using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

/// <summary>
/// Win condition: Deliver the special item to your ship before time runs out
/// Loss condition: run out of time
/// </summary>
public class SpecialItemObjective : TimedObjective {

    public const string type = "SpecialItem";

    private ItemManager itemManager;

    private const float TIMERBAR_WIDTH = 200;

    public SpecialItemObjective() : base()
    {
        itemManager = GameObject.Find("ObjectManager").GetComponent<ItemManager>();
    }

    public override int NumSpecialItems { get { return 1; } }

    public override string Type { get { return type; } }

    public override bool ObjectiveComplete()
    {
        return (itemManager.GetSpecialItemsRemaining() == 0) && 
               (!InventoryScript.inventory.HasItemOfType("SpecialItem"));
    }

    public override bool ObjectiveFailed()
    {
        UpdateTimer();

        if (timeRemaining <= 0.0f) {
            return true;
        }
        return false;
    }

    public override string ToString()
    {
        string currentGoal = itemManager.GetSpecialItemsRemaining() > 0 ?
                             "Find the golden idol!" :
                             "Get back to your ship!";
        return currentGoal + "\nTime remaining: " + (int)timeRemaining;
    }
}
