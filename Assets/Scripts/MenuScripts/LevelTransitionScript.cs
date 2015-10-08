using UnityEngine;
using System.Collections;

public class LevelTransitionScript : MonoBehaviour {
    // Use this for initialization
    void Start () {
        Application.LoadLevel (1);
        
    }
    void OnDestroy() {
        print ("switching levels");
    }
}
