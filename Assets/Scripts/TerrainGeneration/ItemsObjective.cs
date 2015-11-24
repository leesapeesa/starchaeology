using UnityEngine;
using UnityEngine.UI;

public class ItemsObjective : Objective {
    /* Win condition: Collect all collectibles
     * Loss condition: None */

    public const string type = "Items";

    private ItemManager itemManager;

	public ItemsObjective () : base() {
        itemManager = GameObject.Find("ObjectManager").GetComponent<ItemManager>();
        //Text text = GameObject.Find("Timer").GetComponent<Text>();
        //text.enabled = false;
    }

    public override string Type { get { return type; } }

    public override bool ObjectiveComplete() {
        if (itemManager.allCollected == true) {
            return true;
        }
        return false;
    }

    public override bool ObjectiveFailed() {
        // Currently no way of failing this objective
        return false;
    }

    public override string ToString()
    {
        return "Collect all the items!";
    }

}
