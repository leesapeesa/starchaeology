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
        timeRemaining = timeLimit - Time.timeSinceLevelLoad - PersistentLevelSettings.settings.savedTime + player.extraTime;
        int timeAsInt = (int)timeRemaining;
        remainingTimeText.text = timeAsInt.ToString();
        //update the remaining time bar
        float fracTime = timeRemaining / timeLimit;
        float newWidth = fracTime * TIMERBAR_WIDTH;
        timerBar.sizeDelta = new Vector2(newWidth, timerBar.sizeDelta.y);
        timerBar.anchoredPosition = new Vector2(newWidth / 2, 0);
    }
}
