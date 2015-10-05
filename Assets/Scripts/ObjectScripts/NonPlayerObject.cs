using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class NonPlayerObject : MonoBehaviour { // Eventually make abstract.

	private PersistentSettings settings;

	protected void Start() { // Eventually make virtual
		settings = GameObject.FindWithTag("All Settings").GetComponent<PersistentSettings>();
		Assert.IsNotNull (settings);
		Assert.IsNotNull (settings.ptSettings);
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag ("TriggerBounds")) {
			Destroy(gameObject);
			print ("weeeeee");
		}
	}

	private void OnDestroy() {
		if (GameObject.Find ("ObjectManager") == null) {
			// ObjectManager might be removed first.
			return;
		}
		ItemManager itemManager = GameObject.Find ("ObjectManager").GetComponent<ItemManager> ();
		itemManager.Remove (this);
	}
}
