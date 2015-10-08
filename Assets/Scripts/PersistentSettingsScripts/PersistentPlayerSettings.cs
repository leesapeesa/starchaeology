using UnityEngine;
using System.Collections;

public class PersistentPlayerSettings : MonoBehaviour {

	public static PersistentPlayerSettings settings;
	void Awake () {
		if (settings == null) {
			DontDestroyOnLoad (gameObject);
			settings = this;
		} else if (settings != this) {
			Destroy (gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
