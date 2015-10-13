using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimerObjective : Objective { 
    /* Win condition: Reach a score of 10
     * Loss condition: Run out of time */

    public float time = 120; // Countdown time, in seconds
    public int score = 0;

    private int winningScore = 10;
    private bool objectiveComplete = false;
    private Text text;

    public TimerObjective() { 
        text = GameObject.Find("Timer").GetComponent<Text>();
        text.enabled = true;
	}

    public override bool ObjectiveComplete () {
        // Temporary method of increasing score
        if (Input.GetKeyDown(KeyCode.M)) {
            score += 10;
        }

        if (score >= winningScore) {
            return true;
        }
        return false;
    }

    public override bool ObjectiveFailed () {
        time -= Time.deltaTime;
        int timeAsInt = (int)time;
        text.text = timeAsInt.ToString();

        if (time <= 0.0f) {
            return true;
        }
        return false;
    }
}
