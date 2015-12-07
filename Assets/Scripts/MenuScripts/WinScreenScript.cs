using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WinScreenScript : LevelEndScreen {

    public Text timeBonusText;
    public Text overallScoreText;

    public void NextLevel()
    {
        Time.timeScale = 1;
        GameObject.FindWithTag("LoadGame").GetComponent<NewGameScript>().NextLevel();
    }

    private void start() {
        PersistentPlayerSettings.settings.overallScore += (120 - (int)PersistentLevelSettings.settings.savedTime) / 10;
        timeBonusText.text = "Time Bonus: " + PersistentPlayerSettings.settings.overallScore.ToString();
        overallScoreText.text = "Overall Score: " +  ((120 - (int)PersistentLevelSettings.settings.savedTime) / 10).ToString();
    }
}
