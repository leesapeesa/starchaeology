using UnityEngine;
using System.Collections;

// Move this code to the appropriate location after we figure out where it goes.
public class ItemGeneration : MonoBehaviour {

	public int boxCount = 5;
	public int collectCount = 10;

	public Transform bouncyBox;
	public Transform stickyBox;
	public Transform collect;

	private float sideLength = 25; // hard coded in for now.
	// Use this for initialization
	void Awake() {
		addBoxes ();
		addCollectibles ();
		addSticky ();
	}
	private void addBoxes(){
		for (int i = 0; i < boxCount; ++i) {
			Vector3 position = new Vector3(Random.Range(-sideLength, sideLength), 2);
			Instantiate(bouncyBox, position, Quaternion.identity);
		}
	}
	private void addCollectibles() {
		for (int i = 0; i < collectCount; ++i) {
			Vector3 position = new Vector3(Random.Range(-sideLength, sideLength), 5);
			Instantiate(collect, position, Quaternion.identity);
		}
	}
	private void addSticky() {
		for (int i = 0; i < boxCount; ++i) {
			Vector3 position = new Vector3(Random.Range(-sideLength, sideLength), 5);
			Instantiate(stickyBox, position, Quaternion.identity);
		}
	}
}
