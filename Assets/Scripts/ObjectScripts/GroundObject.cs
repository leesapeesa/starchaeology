using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using System;

public class GroundObject : MonoBehaviour {

    private float edge;
    
    void Start() {
        // Get the edge when its first created.
        edge = PersistentTerrainSettings.settings.sideLength / 2;
    }

    void Update() {
        // We want the objects to wrap around the edge of the terrain instead of
        // falling off the edge. Since placing it right at the edge might cause it
        // to trigger again, we manually move it inward a little.
        if (Math.Abs(transform.position.x) > edge) {
            // Which side of the platform its on.
            float sign = Math.Abs(transform.position.x) / transform.position.x;
            // Get height of terrain at the edge we want to appear at so we don't accidentally
            // miss it.
            Vector2[] heights = GameObject.FindObjectOfType<TerrainCreator> ().getPathHeights();

            float newX = sign * (-edge + 2);
            transform.position = new Vector3 (newX, heights[(int)(newX + edge)].y + PersistentTerrainSettings.settings.height + 5, 0);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        // Add things here if you want objects to do things when
        // it hits other things.
        if (other.CompareTag ("TriggerBounds")) {
            print ("Falling off the edge. Really shouldn't be anymore though...");
        }
    }

    void OnDestroy() {
        // set the gravity effect back to default
        if (gameObject.GetComponent<Rigidbody2D> () != null)
            gameObject.GetComponent<Rigidbody2D> ().gravityScale = 1f;
    }
}
