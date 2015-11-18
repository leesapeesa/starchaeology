using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;

public class LoadGameScript : MonoBehaviour {

    private SaveLoadMenuScript loadGameMenu;

    void Start()
    {
        loadGameMenu = gameObject.GetComponent<SaveLoadMenuScript>();
    }

    /// <summary>
    /// Catches the load button click event and triggers a game load from the currently
    /// selected slot.
    /// </summary>
    public void OnLoadButtonPress()
    {
        int? selectedSlot = loadGameMenu.GetActiveToggle();

        //Load the game from the selected slot
        if (selectedSlot.HasValue)
            Load(selectedSlot.Value);
    }

    /// <summary>
    /// Load a saved game from the currently selected slot
    /// </summary>
    public void Load(int slotId)
    {
        //can't load from an empty slot
        if (!PlayerPrefs.HasKey("gameName" + slotId))
            return;

        PersistentLevelSettings.settings.difficulty.LoadDifficulty(slotId);
        PersistentLevelSettings.settings.LoadLevelSettings(slotId);
        PersistentPlayerSettings.settings.LoadPlayerSettings(slotId);
        PersistentTerrainSettings.settings.LoadTerrainSettings(slotId);
        InventoryScript.inventory.LoadInventory(slotId);
        //Now that we have reloaded saved settings, we can use them to reconstruct
        //all remaining settings.
        NewGameScript.LoadLevelSettings();
        //Now, switch over to the level scene
        Application.LoadLevel(3);
    }
}
