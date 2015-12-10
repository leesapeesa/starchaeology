using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WinScreenScript : LevelEndScreen {

    public Text timeBonusText;
    public Text overallScoreText;

    private void OnEnable() {
        int timeBonus = (int)(PersistentLevelSettings.settings.savedTime / 10);
        print("ASDFJKASLJKDFHASHDFJKLASDFHJKLASDHHLASDFH");
        print(timeBonus);
        print(PersistentPlayerSettings.settings.overallScore);
      
        timeBonusText.text = "Time Bonus: " + timeBonus;
        PersistentPlayerSettings.settings.overallScore += timeBonus;
        overallScoreText.text = "Score: " + PersistentPlayerSettings.settings.overallScore.ToString();
    }

    public void NextLevel()
    {
        Time.timeScale = 1;
        GameObject.FindWithTag("LoadGame").GetComponent<NewGameScript>().NextLevel();
    }
}
