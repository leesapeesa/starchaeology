using UnityEngine;
using System.Collections;

public class WinScreenScript : LevelEndScreen {

    public void NextLevel()
    {
        Time.timeScale = 1;
        GameObject.FindWithTag("LoadGame").GetComponent<NewGameScript>().NextLevel();
    }
}
