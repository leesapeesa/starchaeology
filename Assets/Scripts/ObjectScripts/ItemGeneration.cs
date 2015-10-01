using UnityEngine;
using System.Linq;
using System.Collections;

// Move this code to the appropriate location after we figure out where it goes.
public class ItemGeneration : MonoBehaviour {

	public int boxCount = 5;
	public int collectCount = 10;

	public Transform bouncyBox;
	public Transform stickyBox;
	public Transform collect;

	private float sideLength = 25f;
	private PersistentSettings settings;
	private float closestToEdge = 5f;
	private float[] heights;
	private Transform[] collectibles;
	// Use this for initialization
	void Awake() {
		settings = GameObject.FindWithTag("All Settings").GetComponent<PersistentSettings>();
		heights = GameObject.FindObjectOfType<TerrainCreator> ().GetHeights();
		sideLength = settings.ptSettings.sideLength - closestToEdge;

		collectibles = new Transform[collectCount];
		addCollectibles ();
		addBoxes ();

		addSticky ();
	}

	void Update() {
		if (CollectedAll ())
			print ("YAY");
	}

	private bool CollectedAll() {
		bool allDisabled = true;
		for (int i = 0; i < collectCount; ++i) {
			allDisabled &= !collectibles[i].gameObject.activeSelf;
			if (!allDisabled)
				return false;
		}
		return true;
	}

	private void addBoxes(){
		for (int i = 0; i < boxCount; ++i) {
			Vector3 position = new Vector3(Random.Range(-sideLength / 2, sideLength / 2), 2);
			Instantiate(bouncyBox, position, Quaternion.identity);
		}
	}
	private void addCollectibles() {
		for (int i = 0; i < collectCount; ++i) {
			float xCoor = Random.Range(0, sideLength);
			float height = heights[(int)xCoor] + Random.Range (0, 3);
			height = Mathf.Max(height, 5);
			Vector3 position = new Vector3(xCoor - sideLength / 2, height);
			collectibles[i] =Instantiate(collect, position, Quaternion.identity) as Transform;
		}
	}
	private void addSticky() {
		for (int i = 0; i < boxCount; ++i) {
			Vector3 position = new Vector3(Random.Range(-sideLength / 2, sideLength / 2), 5);
			Instantiate(stickyBox, position, Quaternion.identity);
		}
	}
}
