using UnityEngine;

/// <summary>
/// Static class containing helper functions for manipulating levels
/// </summary>
public static class LevelUtil {

    /// <summary>
    /// Specifies the scene to return to when ending a game
    /// </summary>
    public enum GameEndType
    {
        NEW_GAME = 2,   //go back to the new game screen
        MAIN_MENU = 0   //go back to the main menu
    }

    /// <summary>
    /// End the current game. Perform cleanup tasks and then return to the scene specified by type
    /// </summary>
    public static void EndGame(GameEndType type)
    {
        //Destroy all persistent settings objects
        Object.Destroy(PersistentLevelSettings.settings.gameObject);
        Object.Destroy(PersistentPlayerSettings.settings.gameObject);
        Object.Destroy(PersistentTerrainSettings.settings.gameObject);
        Object.Destroy(InventoryScript.inventory.gameObject);

        //Now, load the desired scene
        Application.LoadLevel((int)type);
    }
	
}
