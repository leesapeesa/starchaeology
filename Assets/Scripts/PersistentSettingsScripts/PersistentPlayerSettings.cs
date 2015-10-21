using UnityEngine;
using System.Collections;

public class PersistentPlayerSettings : MonoBehaviour {

    public static PersistentPlayerSettings settings;

    public float health;

    void Awake () {
        if (settings == null) {
            DontDestroyOnLoad (gameObject);
            SetDefault();
            settings = this;
        } else if (settings != this) {
            Destroy (gameObject);
        }
    }
    
    // Update is called once per frame
    void Update () {
    
    }

    private void SetDefault()
    {
        health = PlayerCharacter2D.MAX_HEALTH;
    }
}
