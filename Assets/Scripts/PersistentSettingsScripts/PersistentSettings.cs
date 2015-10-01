using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class PersistentSettings : MonoBehaviour {

	public PersistentTerrainSettings ptSettings;
	void Start () {
		Assert.IsNotNull (ptSettings);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
