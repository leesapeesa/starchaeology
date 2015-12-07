using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelTransitionScript : MonoBehaviour {
    // Use this for initialization
    //public Texture emptyProgressBar;
    //public Texture fullProgressBar;
    public Font font;
    public float maxGravity = 2f;
    private RectTransform progressBar;
    private Text progressText;
    private Text blurbText;
    private const float PROGRESSBAR_WIDTH = 200f;

    private AsyncOperation async = null;
    private Fortunes fortunes;

    private bool anyKeyToContinue;

    void Start() {
        progressBar = GameObject.Find("CurrentProgress").GetComponent<RectTransform>();
        progressText = GameObject.Find ("LoadingProgress").GetComponent<Text> ();
        blurbText = GameObject.Find ("BlurbText").GetComponent<Text> ();
        anyKeyToContinue = false;
        // First time from the main menu PLS doesn't exist yet.
        if (PersistentLevelSettings.settings != null) {
            fortunes = PersistentLevelSettings.settings.fortunes;
            GetRandomBlurb ();
        }
        StartCoroutine (DisplayLoadingScreen ());
        SetMaximumGravity();
    }

    void Update() {
        // Allows user to hit any key to continue onto level.
        if (Input.anyKey && anyKeyToContinue) {
            async.allowSceneActivation = true;
        }
    }

    void SetMaximumGravity() {
        float playerHeight = 3.7f;
        maxGravity = 5.0f;
        while (apex() < playerHeight ) {
            maxGravity -= 0.05f;
        }
    }

    private float apex() {
        // Simple mechanics: maxHeight = v_0^2 / (2 * g) + y_0
        // where v_0 = (jumpForce - gravityEffect), g = gravityEffect and y_0 = terrainHeight.
        float approxLengthOfOneFrame = Time.fixedDeltaTime;
        float gravity = maxGravity * 9.8f;
        float initialVelocity = (PersistentPlayerSettings.settings.jumpForce - gravity) * approxLengthOfOneFrame;
        return initialVelocity * initialVelocity / (2 * gravity);
    }

    private void GetRandomBlurb() {
        blurbText.text = fortunes.GetRandom();
    }

    IEnumerator DisplayLoadingScreen() {
        // Drawing the Loading progress:
        async = Application.LoadLevelAsync (1);
        async.allowSceneActivation = false;

        // While there is still part of the level to load, update the progress bar.
        // Since we don't want the scene to immediately activate, we want the user to
        // choose when to continue, the maximum async.progress gives is 0.9.
        while (async.progress < 0.9f) {
            // artificial loading.
            yield return null;
            print(async.progress);
            // The maximum it is going to let us have as progress is 0.9.
            float fracHealth = (float)(async.progress / 0.9);
            float newWidth = fracHealth * PROGRESSBAR_WIDTH;
            progressBar.sizeDelta = new Vector2(newWidth, progressBar.sizeDelta.y);
            progressBar.anchoredPosition = new Vector2(newWidth / 2, 0);
            progressText.text = "Progress: " + Mathf.Round ((float) (async.progress / 0.9 * 100)).ToString() + " %";
        }

        // Once the next level is loaded, let the user press the start button.
        GameObject.Find("Start Level").GetComponent<Button>().interactable = true;
        anyKeyToContinue = true;
    }

    // Method for button listener to call in Unit.
    public void OnButtonClick() {
        async.allowSceneActivation = true;
    }

    void OnDestroy() {
        print ("switching levels");
        //Only reset parameters if we are doing a normal level load, not restoring a savegame
        if (!PersistentLevelSettings.settings.loadFromSave) {
            PersistentTerrainSettings.settings.gravityEffect = Random.Range(0.7f, maxGravity);
            if (PersistentPlayerSettings.settings == null) {
                // It will be null if we're loading the level from the New Game screen for the
                // first time.
                print("PersistentPlayerSettings doesn't exist");
                return;
            }
            PersistentPlayerSettings.settings.levelScore = 0;
            PersistentLevelSettings.settings.enemyBehavior = GroundPathEnemy.Behavior.NORMAL;
        }
    }
}
