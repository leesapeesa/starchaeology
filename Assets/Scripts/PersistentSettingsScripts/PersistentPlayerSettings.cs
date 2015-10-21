using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PersistentPlayerSettings : MonoBehaviour {

    public static PersistentPlayerSettings settings;
    public Text scoreText;
    public Text healthText;

    private InventoryScript inventory;
    private int score = 0;
    private int health = 100;

    void Awake () {
        if (settings == null) {
            DontDestroyOnLoad (gameObject);
            settings = this;
        } else if (settings != this) {
            Destroy (gameObject);
        }
    }
    void Start() {
        inventory = GameObject.Find ("Inventory").GetComponent<InventoryScript> ();
        score = 0;
        health = 100;
        scoreText.text = "Score: 0";
        healthText.text = "Health: 100";
    }

    public void CollectedItem(Collectible item) {
        // Don't put scoring items into the inventory.
        if (item.function == ItemFunction.Score) {
            score += item.value;
            scoreText.text = "Score: " + score.ToString ();
            print ("Score: " + score);
            return;
        }
        inventory.AddItemToInventory (item);
    }

    public void UseItem(Collectible item) {
        if (item.function == ItemFunction.Health) {
            health += item.value; // TODO: After we have something to decrease health, make it capped.
            healthText.text = "Health: " + health.ToString();
            print ("Health increased: " + health.ToString ());
        }
        print ("Used item: " + item.type);
    }
}
