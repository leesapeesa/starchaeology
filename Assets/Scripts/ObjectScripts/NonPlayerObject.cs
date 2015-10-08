using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class NonPlayerObject : MonoBehaviour { // Eventually make abstract.

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag ("TriggerBounds")) {
            Destroy(gameObject);
            print ("weeeeee");
        }
    }

    void OnDestroy() {
        if (GameObject.Find ("ObjectManager") == null) {
            // ObjectManager might be removed first.
            return;
        }
        ItemManager itemManager = GameObject.Find ("ObjectManager").GetComponent<ItemManager> ();
        itemManager.RemoveFromScene (this);
    }
}
