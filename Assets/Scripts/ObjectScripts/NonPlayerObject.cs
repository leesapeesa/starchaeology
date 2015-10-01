using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class NonPlayerObject : MonoBehaviour { // Eventually make abstract.

	private PersistentSettings settings;
	private float maxFromEdge = 2f;

	protected void Start() { // Eventually make virtual
		settings = GameObject.FindWithTag("All Settings").GetComponent<PersistentSettings>();
		Assert.IsNotNull (settings);
		Assert.IsNotNull (settings.ptSettings);
	}

	protected void RemoveObject() {
		gameObject.SetActive (false);
	}

	private void Update() {
		if (FallenOff ()) {
			print ("Weeeee");
			RemoveObject ();
		}
	}

	private bool FallenOff() {
		Vector3 position = transform.position;
		Vector3 terrainOrigin = settings.ptSettings.terrainPosition;
		float sideLength = settings.ptSettings.sideLength;
		// Since terrain is offset by half of the side length, we need to do the appropriate calculations for it.
		return (position.x < (terrainOrigin.x - sideLength / 2f - maxFromEdge) ||
		       (position.x > (terrainOrigin.y + sideLength / 2f + maxFromEdge)));
	}
}
