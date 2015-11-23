using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PersistentPlayerSettings : MonoBehaviour {

    public static PersistentPlayerSettings settings;

    private InventoryScript inventory;
    public int overallScore = 0; // Score for the overall game.
    public int levelScore = 0; // Score for the level.
    public float health = 100f;
    public float jumpForce = 400f;
    public Vector2 playerPos;
    public float extraTime;
    public string playerName;

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
        jumpForce = 400f;
        playerPos = Vector2.zero;
        if (inventory)
            inventory.EmptyInventory ();
    }

    /// <summary>
    /// Save any relevant player settings that cannot be reconstructed from the saved difficulty
    /// </summary>
    public void SavePlayerSettings(int slotId)
    {
        PlayerPrefs.SetInt("levelScore" + slotId, levelScore);
        PlayerPrefs.SetFloat("health" + slotId, health);
        PlayerPrefs.SetFloat("playerx" + slotId, playerPos.x);
        PlayerPrefs.SetFloat("playery" + slotId, playerPos.y);
        PlayerPrefs.SetFloat("extraTime" + slotId, extraTime);
        PlayerPrefs.SetString("playerName" + slotId, playerName);
    }

    /// <summary>
    /// Load all saved player settings from the specified save slot
    /// </summary>
    public void LoadPlayerSettings(int slotId)
    {
        levelScore = PlayerPrefs.GetInt("levelScore" + slotId);
        health = PlayerPrefs.GetFloat("health" + slotId);
        playerPos.x = PlayerPrefs.GetFloat("playerx" + slotId);
        playerPos.y = PlayerPrefs.GetFloat("playery" + slotId);
        extraTime = PlayerPrefs.GetFloat("extraTime" + slotId);
        playerName = PlayerPrefs.GetString("playerName" + slotId);
    }

    void OnDestroy() {
        print ("Persistent Player Settings Destroyed");
        SetDefault ();
    }
}
