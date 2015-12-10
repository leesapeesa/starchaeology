using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndMenuScript : MonoBehaviour {
    public Text scoreText;
    public Text previousHighScoreText;
    public Text newHighScoreText;

    public void NewGame() {
        Time.timeScale = 1;
        LevelUtil.EndGame(LevelUtil.GameEndType.NEW_GAME);
    }

    private void Start() {
        int highScore = 0;
        int previousHighscore = 0;

        scoreText.text = "Score: " + PersistentPlayerSettings.settings.overallScore.ToString();

        if (PlayerPrefs.HasKey("HighScore")) {
            highScore = PlayerPrefs.GetInt("HighScore");
        } else {
            highScore = 0;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        if (highScore < PersistentPlayerSettings.settings.overallScore) {

            previousHighscore = highScore;
            PlayerPrefs.SetInt("HighScore", PersistentPlayerSettings.settings.overallScore);
            newHighScoreText.text = "Congratulations, you've achieved a new high score!";
            previousHighScoreText.text = "Previous high score: " + previousHighscore.ToString();
        }
    }

	public void MainMenu()
    {
        LevelUtil.EndGame(LevelUtil.GameEndType.MAIN_MENU);
    }
}
