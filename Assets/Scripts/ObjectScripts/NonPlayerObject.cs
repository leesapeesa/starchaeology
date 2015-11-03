using UnityEngine;

public abstract class NonPlayerObject : MonoBehaviour {

    void OnDestroy() {
        // Object Manager keeps track of items so the default OnDestroy will remove the object
        // from ObjectManager in case our manager wants to do something with the object.
        if (GameObject.Find ("ObjectManager") == null) {
            // ObjectManager might be removed first.
            return;
        }
        ItemManager itemManager = GameObject.Find ("ObjectManager").GetComponent<ItemManager> ();
        itemManager.RemoveFromScene (this);
    }
}
