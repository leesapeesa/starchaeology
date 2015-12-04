using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WinScreenScript : LevelEndScreen {

    public void NextLevel()
    {
        Time.timeScale = 1;
        GameObject.FindWithTag("LoadGame").GetComponent<NewGameScript>().NextLevel();
    }
}
