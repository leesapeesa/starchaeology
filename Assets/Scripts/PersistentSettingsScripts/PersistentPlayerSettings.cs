using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PersistentPlayerSettings : MonoBehaviour {

    public static PersistentPlayerSettings settings;

    private InventoryScript inventory;
    public int overallScore = 0; // Score for the overall game.
    public int levelScore = 0; // Score for the level.
    public float health = 100f;

    void Awake () {
        if (settings == null) {
            DontDestroyOnLoad (gameObject);
            inventory = InventoryScript.inventory;
            SetDefault ();
            settings = this;
        } else if (settings != this) {
            Destroy (gameObject);
        }

    }

    public void SetDefault()
    {
        levelScore = 0;
        overallScore = 0;
        health = PlayerCharacter2D.MAX_HEALTH;
        if (inventory)
            inventory.EmptyInventory ();
    }

    void OnDestroy() {
        print ("Persistent Player Settings Destroyed");
        SetDefault ();
    }
}
