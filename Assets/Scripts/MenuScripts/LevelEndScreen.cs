using UnityEngine;
using System.Collections;

public class LevelEndScreen : MonoBehaviour {

    public void NewGame()
    {
        Time.timeScale = 1;
        LevelUtil.EndGame(LevelUtil.GameEndType.NEW_GAME);
    }
}
