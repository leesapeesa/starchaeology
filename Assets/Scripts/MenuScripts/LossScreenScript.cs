using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LossScreenScript : LevelEndScreen {
    public Text scoreText;
    public Text newHighScoreText;
    public Text previousHighScoreText;

    private int highScore;

    private void OnEnable () {
        print("HERHEHREHRHERHEHRHEHREHRHHERHHEHE");
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
}
