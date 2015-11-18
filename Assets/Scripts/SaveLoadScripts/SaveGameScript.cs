using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections.Generic;

/// <summary>
/// Handles logic for saving the current game state to permanent storage
/// </summary>
public class SaveGameScript : MonoBehaviour {

    private ToggleGroup toggles;
    private TerrainCreator terrain;
    private ItemManager itemManager;
    private LevelScript level;
    private SaveLoadMenuScript saveGameMenu;

    void Start()
    {
        toggles = GetComponent<ToggleGroup>();
        terrain = GameObject.FindGameObjectWithTag("Terrain").GetComponent<TerrainCreator>();
        itemManager = GameObject.FindGameObjectWithTag("ObjectManager").GetComponent<ItemManager>();
        level = GameObject.FindGameObjectWithTag("LevelObject").GetComponent<LevelScript>();
        saveGameMenu = gameObject.GetComponent<SaveLoadMenuScript>();
        Assert.IsNotNull(toggles);
    }

    /// <summary>
    /// Catches the save button click event and triggers a game save to the currently
    /// selected slot.
    /// </summary>
    public void OnSaveButtonPress()
    {
        int? selectedSlot = saveGameMenu.GetActiveToggle();

        //Save the game to the selected slot
        if (selectedSlot.HasValue)
            Save(selectedSlot.Value);
    }

    /// <summary>
    /// Saves the current game to the save slot specified by slotId
    /// </summary>
	public void Save(int slotId)
    {
        //TODO: we should warn the player if they are overwriting another saved game. JPC 11/13/15
        try {
            PlayerPrefs.SetString("gameName" + slotId, System.DateTime.Now.ToString());
            PersistentLevelSettings.settings.difficulty.SaveDifficulty(slotId);
            PersistentLevelSettings.settings.SaveLevelSettings(slotId);
            PersistentPlayerSettings.settings.SavePlayerSettings(slotId);
            PersistentTerrainSettings.settings.SaveTerrainSettings(slotId);
            InventoryScript.inventory.SaveInventory(slotId);
            terrain.SaveTerrain(slotId);
            itemManager.SaveItems(slotId);
            level.SaveLevel(slotId);
            PlayerPrefs.Save();
            print("Finished save");
            saveGameMenu.RefreshText();
        } catch(PlayerPrefsException e) {
            print("Out of space, got exception " + e);
        }
    }
}
