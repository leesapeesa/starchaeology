﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelScript : MonoBehaviour {

    public GameObject level;
    public bool objectiveCompleted = false;
    public bool objectiveFailed = false;
    public Objective objective;

    private int numObjectives = 2;
    private Text objectivesText;
    private PlayerCharacter2D player;
    private RectTransform healthBar;
    private const float HEALTHBAR_WIDTH = 200f;

    private void Start() {
        CreateRandomObjective();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerCharacter2D>();
        objectivesText = GameObject.Find("ObjectivesText").GetComponent<Text>();
        healthBar = GameObject.Find("CurrentHealth").GetComponent<RectTransform>();
    }

    private void CreateRandomObjective () {
        float random = Random.value;
        int objectiveToChoose = (int)(Random.value * numObjectives);

        if (objectiveToChoose == 0) {
            objective = new ItemsObjective();
        } else {
            objective = new TimerObjective();
        }
    }

    private void Update() {

        //Update health display
        float fracHealth = player.Health() / PlayerCharacter2D.MAX_HEALTH;
        float newWidth = fracHealth * HEALTHBAR_WIDTH;
        healthBar.sizeDelta = new Vector2(newWidth, healthBar.sizeDelta.y);
        healthBar.anchoredPosition = new Vector2(newWidth / 2, 0);

        if (objective != null) {
            objectiveCompleted = objective.ObjectiveComplete();
            objectiveFailed = objective.ObjectiveFailed();
            //Update the displayed objectives text
            objectivesText.text = objective.ToString();
        }

        if (objectiveCompleted && !objectiveFailed) {
            Canvas winScreen = GameObject.Find("WinScreen").GetComponent<Canvas>();
            winScreen.enabled = true;
            print("You Win! Yay!");
        } else if (objectiveFailed && !objectiveCompleted) {
            Canvas lossScreen = GameObject.Find("LossScreen").GetComponent<Canvas>();
            lossScreen.enabled = true;
            print("You died. Better luck next time");
        }
    }
}
