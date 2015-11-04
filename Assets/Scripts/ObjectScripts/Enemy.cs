using UnityEngine;
using System.Collections;

public class Enemy : NonPlayerObject {

    protected float minX = -50f;
    protected float maxX = 50f;

    // Use this for initialization
    protected void Start () {
        maxX = PersistentTerrainSettings.settings.sideLength / 2;
        minX = -maxX;
	}

}
