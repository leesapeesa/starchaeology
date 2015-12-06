using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class TimedObjective : Objective {

    public float timeLimit = 120; // Countdown time, in seconds
    public float timeRemaining;

    private RectTransform timerBar;
    private Text remainingTimeText;
    private PlayerCharacter2D player;

    private const float TIMERBAR_WIDTH = 200;

    public TimedObjective() : base()
    {
        timerBar = GameObject.Find("RemainingTime").GetComponent<RectTransform>();
        remainingTimeText = GameObject.Find("TimerText").GetComponent<Text>();
        player = GameObject.Find("Player").GetComponent<PlayerCharacter2D>();
        GameObject.Find("TimerBar").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        timeRemaining = timeLimit;
    }

    /// <summary>
    /// Compute the remaining time and update the timer UI
    /// </summary>
    protected void UpdateTimer()
    {
        capPlayerTime();
        timeRemaining = timeLimit - Time.timeSinceLevelLoad - PersistentLevelSettings.settings.savedTime + player.extraTime;
        int timeAsInt = (int)timeRemaining;
        remainingTimeText.text = timeAsInt.ToString();
        //update the remaining time bar
        float fracTime = timeRemaining / timeLimit;
        float newWidth = fracTime * TIMERBAR_WIDTH;
        timerBar.sizeDelta = new Vector2(newWidth, timerBar.sizeDelta.y);
        timerBar.anchoredPosition = new Vector2(newWidth / 2, 0);
    }

    /// <summary>
    /// If the player's current extra time would result in having more time left than the maximum time limit,
    /// adjust the player's extra time to cap the possible amount of time left.
    /// </summary>
    protected void capPlayerTime()
    {
        float rawTimeRemaining = timeLimit - Time.timeSinceLevelLoad - PersistentLevelSettings.settings.savedTime + player.extraTime;
        if (rawTimeRemaining > timeLimit) {
            //set extra time such that it exactly cancels out already passed time, such that time remaining equals timeLimit
            player.extraTime = Time.timeSinceLevelLoad + PersistentLevelSettings.settings.savedTime;
        }
    }
}
