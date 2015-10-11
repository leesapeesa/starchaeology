using UnityEngine;
using System.Collections;

public class LevelScript : MonoBehaviour {

    public GameObject level;
    public bool objectiveCompleted = false;
    public bool objectiveFailed = false;
    public Objective objective;

    private int numObjectives = 2;

    private void Start() {
        CreateRandomObjective();
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
        if (objective != null) {
            objectiveCompleted = objective.ObjectiveComplete();
            objectiveFailed = objective.ObjectiveFailed();
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
