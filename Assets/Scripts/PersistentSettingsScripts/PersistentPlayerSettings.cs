using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PersistentPlayerSettings : MonoBehaviour {

    public static PersistentPlayerSettings settings;
    public Text scoreText;
    public Text healthText;

    private InventoryScript inventory;
    public int score = 0;
    public float health = 100f;

    void Awake () {
        if (settings == null) {
            DontDestroyOnLoad (gameObject);
            SetDefault();
            settings = this;
        } else if (settings != this) {
            Destroy (gameObject);
        }
    }
    void Start() {
        inventory = GameObject.Find ("Inventory").GetComponent<InventoryScript> ();
        score = 0;
        health = 100;
        //scoreText.text = "Score: 0";
    }

    private void SetDefault()
    {
        health = PlayerCharacter2D.MAX_HEALTH;
    }
}
