using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelScript : MonoBehaviour {

    public GameObject level;
    public bool objectiveCompleted = false;
    public bool objectiveFailed = false;
    public Objective objective;

    private int numObjectives = 3;
    private Text objectivesText;
    private PlayerCharacter2D player;
    private ItemManager itemManager;
    private RectTransform healthBar;
    private const float HEALTHBAR_WIDTH = 200f;
    private Text scoreText;

    private void Start() {
        Time.timeScale = 1;
        CreateRandomObjective();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerCharacter2D>();
        objectivesText = GameObject.Find("ObjectivesText").GetComponent<Text>();
        healthBar = GameObject.Find("CurrentHealth").GetComponent<RectTransform>();
        scoreText = GameObject.Find ("Score").GetComponent<Text> ();
        itemManager = GameObject.Find("ObjectManager").GetComponent<ItemManager>();
        itemManager.InitializeItems(objective);
    }

    private void CreateRandomObjective () {
        float random = Random.value;
        float objectiveToChoose = Random.value * numObjectives;

        if (objectiveToChoose < 1) {
            objective = new ItemsObjective();
        } else if (objectiveToChoose < 2) {
            objective = new TimerObjective();
        } else {
            objective = new SpecialItemObjective();
        }
    }

    private void Update() {

        //Update health display
        float fracHealth = player.health / PlayerCharacter2D.MAX_HEALTH;
        float newWidth = fracHealth * HEALTHBAR_WIDTH;
        healthBar.sizeDelta = new Vector2(newWidth, healthBar.sizeDelta.y);
        healthBar.anchoredPosition = new Vector2(newWidth / 2, 0);

        // Update score display
        scoreText.text = "Score: " + PersistentPlayerSettings.settings.levelScore.ToString ();

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
            Time.timeScale = 0;
        } else if ((objectiveFailed && !objectiveCompleted) || player.health <= 0) {
            Canvas lossScreen = GameObject.Find("LossScreen").GetComponent<Canvas>();
            lossScreen.enabled = true;
            print("You died. Better luck next time");
            Time.timeScale = 0;
        }
    }
}
