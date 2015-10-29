using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class NonPlayerObject : MonoBehaviour { // Eventually make abstract.

    private float edge;

    void Start() {
        edge = PersistentTerrainSettings.settings.sideLength / 2;
    }
    void Update() {
        if (transform.position.x > edge) {
            print ("Why did the NPO cross the road???");
            Vector3 oldPos = transform.position;
            // Next round of hours, actually find the height of the terrain at x.
            transform.position = new Vector3 (-edge + 1, 10, oldPos.z);
        }
        else if (transform.position.x < -edge) {
            print ("Why did the NPO cross the road???");
            Vector3 oldPos = transform.position;
            transform.position = new Vector3 (edge - 1, 10, oldPos.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag ("TriggerBounds")) {
            //Destroy(gameObject);
            print ("Really shouldn't be touching this anymore");
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
