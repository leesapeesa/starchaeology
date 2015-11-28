using UnityEngine;
using System.Collections;

public class ItemsObjective : Objective {

    /* Win condition: Collect all collectibles of the specified type
     * Loss condition: None */

    private enum GoalType
    {
        GEM,
        SCROLL,
        BONE
    }

    public const string type = "Items";

    private GoalType goal;
    private string[] goalNames = { "gems", "ancient scrolls", "mysterious bones" };
    private ItemManager itemManager;

    public ItemsObjective() : base() {
        itemManager = GameObject.Find("ObjectManager").GetComponent<ItemManager>();
        goal = (GoalType)Random.Range(0, 2);
    }

    public override string Type { get { return type; } }

    public override int? GoalCollectibleType { get { return (int)goal; } }

    public override bool ObjectiveComplete()
    {
        if (itemManager.allCollected == true) {
            return true;
        }
        return false;
    }

    public override bool ObjectiveFailed()
    {
        // Currently no way of failing this objective
        return false;
    }

    public override string ToString()
    {
        return "Collect all the " + goalNames[(int)goal] + "!";
    }
}
