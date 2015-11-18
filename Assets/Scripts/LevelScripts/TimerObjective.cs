using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class TimerObjective : Objective { 
    /* Win condition: Reach a score of 10
     * Loss condition: Run out of time */

    public float timeLimit = 120; // Countdown time, in seconds
    public float timeRemaining;

    public static int winningScore = 5;
    public const string type = "Timer";

    private bool objectiveComplete = false;
    private Text text;
    private PlayerCharacter2D player;

    public TimerObjective() { 
        text = GameObject.Find("Timer").GetComponent<Text>();
        player = GameObject.Find ("Player").GetComponent<PlayerCharacter2D> ();
        text.enabled = true;
        timeRemaining = timeLimit;
	}

    //make sure there are enough point collectibles spawned to finish this objective
    public override int NumPointItems { get { return winningScore / PointCollectible.points + 1; } }

    public override string Type { get { return type; } }

    public override bool ObjectiveComplete () {
        if (PersistentPlayerSettings.settings.levelScore >= winningScore) {
            Debug.Log ("Objective Complete: " + PersistentPlayerSettings.settings.levelScore);
            return true;
        }
        return false;
    }

    public override bool ObjectiveFailed () {
        timeRemaining = timeLimit - Time.timeSinceLevelLoad - PersistentLevelSettings.settings.savedTime + player.extraTime;
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
