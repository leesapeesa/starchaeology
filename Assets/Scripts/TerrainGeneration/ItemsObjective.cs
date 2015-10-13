using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemsObjective : Objective {
    /* Win condition: Collect all collectibles
     * Loss condition: None */

    private ItemManager itemManager;

	public ItemsObjective () {
        itemManager = GameObject.Find("ObjectManager").GetComponent<ItemManager>();
        Text text = GameObject.Find("Timer").GetComponent<Text>();
        text.enabled = false;
    }

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

}
