using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LossScreenScript : MonoBehaviour {
    public Button newGameButton;

    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
    
    }

    public void NewGame () {
        Time.timeScale = 1;
        Application.LoadLevel(2);
    }

    public void NextLevel() {
        Application.LoadLevel (3);
    }
}
