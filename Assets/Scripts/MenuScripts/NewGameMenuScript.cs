using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class NewGameMenuScript : MonoBehaviour {

    public Button startButton;

    // Use this for initialization
    void Start () {
    }

    // Should be changed to reflect how we end up loading the game
    public void NewGame() {
        Application.LoadLevel(1);
    }


}
