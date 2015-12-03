using UnityEngine;
using System.Collections;

public class EndMenuScript : MonoBehaviour {

	public void MainMenu()
    {
        LevelUtil.EndGame(LevelUtil.GameEndType.MAIN_MENU);
    }
}
