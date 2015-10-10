using UnityEngine;
using System.Collections;

public class ItemsObjective : Objective {

    private bool objectiveComplete = false;
    private ItemManager itemManager;

	// Use this for initialization
	void Start () {
        itemManager = GameObject.Find("ObjectManager").GetComponent<ItemManager>();
	}
	
	// Update is called once per frame
	void Update () {
        if (itemManager.allCollected == true) {
            objectiveComplete = true;
        }
    }

    public override bool ObjectiveComplete() {
        return objectiveComplete;
    }

    public override bool ObjectiveFailed() {
        // Currently no way of failing this objective
        return false;
    }

}
