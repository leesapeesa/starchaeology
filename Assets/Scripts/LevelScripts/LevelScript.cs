using UnityEngine;
using System.Collections;

public class LevelScript : MonoBehaviour {

    public GameObject level;
    public bool objectiveCompleted = false;
    public bool objectiveFailed = false;
    public Objective objective;

    private int numObjectives = 1;

    private void Start() {
        CreateRandomObjective();
    }

    private void CreateRandomObjective () {
        float random = Random.value;
        int objectiveToChoose = (int) (Random.value * numObjectives);

        if (objectiveToChoose == 0) {
            objective = level.AddComponent<ItemsObjective>();
        }
    }

    private void Update() {
        if (objective) {
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
