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
        //If we are loading a saved game, restore the saved goal.
        //Otherwise we can pick one randomly
        if (PersistentLevelSettings.settings.loadFromSave) { 
            int savedType = PlayerPrefs.GetInt("goalItemType" + PersistentLevelSettings.settings.loadSlot);
            if (savedType == -1)
                goal = (GoalType)Random.Range(0, 2);
            else
                goal = (GoalType)savedType;
        } else
            goal = (GoalType)Random.Range(0, 2);
    }

    public override string Type { get { return type; } }

    public override int? GoalCollectibleType { get { return (int)goal; } }

    public override bool ObjectiveComplete()
    {
        return (itemManager.GetGoalItemsRemaining() == 0);
    }

    public override bool ObjectiveFailed()
    {
        // Currently no way of failing this objective
        return false;
    }

    public override string ToString()
    {
        int remaining = itemManager.GetGoalItemsRemaining();
        string currentGoal = remaining > 0 ? 
                             "collect all the " + goalNames[(int)goal] + "! (" + remaining + " remaining)" :
                             RETURN_TO_SHIP;
        return currentGoal;
    }
}
