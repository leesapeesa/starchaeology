using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimerObjective : Objective { 
    /* Win condition: Reach a score of 10
     * Loss condition: Run out of time */

    public float timeLimit = 120; // Countdown time, in seconds
    public float timeRemaining;

    private int winningScore = 10;
    private bool objectiveComplete = false;
    private Text text;
    private PlayerCharacter2D player;

    public TimerObjective() { 
        text = GameObject.Find("Timer").GetComponent<Text>();
        player = GameObject.Find ("Player").GetComponent<PlayerCharacter2D> ();
        text.enabled = true;
        timeRemaining = timeLimit;
	}

    public override bool ObjectiveComplete () {
        if (PersistentPlayerSettings.settings.levelScore >= winningScore) {
            Debug.Log ("Objective Complete: " + PersistentPlayerSettings.settings.levelScore);
            return true;
        }
        return false;
    }

    public override bool ObjectiveFailed () {
        timeRemaining = Mathf.Min (timeLimit, timeLimit - Time.timeSinceLevelLoad + player.extraTime);
        int timeAsInt = (int)timeRemaining;
        text.text = timeAsInt.ToString();

        if (timeRemaining <= 0.0f) {
            return true;
        }
        return false;
    }

    public override string ToString()
    {
        return "Score " + winningScore + " points. \n Time remaining: " + (int)timeRemaining;
    }
}
