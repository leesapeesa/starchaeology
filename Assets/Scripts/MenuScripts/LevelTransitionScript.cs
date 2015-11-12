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
    private const float PROGRESSBAR_WIDTH = 200f;

    private AsyncOperation async = null;

    IEnumerator Load() {
        progressBar = GameObject.Find("CurrentHealth").GetComponent<RectTransform>();
        yield return async;
    }

    void Start() {
        StartCoroutine (DisplayLoadingScreen ());
    }

    IEnumerator DisplayLoadingScreen() {
        // Drawing the Loading progress:
        async = Application.LoadLevelAsync (1);
        async.allowSceneActivation = false;
        while (async.progress < 0.9f) {
            print(async.progress);
            yield return null;
            float fracHealth = async.progress;
            float newWidth = fracHealth * PROGRESSBAR_WIDTH;
            progressBar.sizeDelta = new Vector2(newWidth, progressBar.sizeDelta.y);
            progressBar.anchoredPosition = new Vector2(newWidth / 2, 0);
        }

        GameObject.Find("Start Level").GetComponent<Button>().interactable = true;
    }
    /*
    void OnGUI() {
        int counter = 1000;
        while (!async.isDone || counter < 0) {
            int width = Camera.main.pixelWidth;
            int height = Camera.main.pixelHeight;
            // Eventually have a loading bar.
            // GUI.DrawTexture(new Rect(width / 2 - 50, height / 2 - 50, 100, 50), emptyProgressBar);
            // GUI.DrawTexture(new Rect(width / 2 - 50, height / 2 - 50, 100 * async.progress, 50), fullProgressBar);
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.skin.font = font;
            GUIStyle myStyle = new GUIStyle (GUI.skin.label);
            myStyle.fontSize = 30;
            GUI.Label (new Rect (width / 2 - 50, height / 2 - 50, 100, 50), string.Format ("{0:N0}%", async.progress * 100f), myStyle);
            print (async.progress);
            yield return null;
            counter--;
        }
        GameObject.Find("Start Level").GetComponent<Button>().interactable = true;
    }
*/


    public void OnButtonClick() {
        async.allowSceneActivation = true;

    }
    void OnDestroy() {
        print ("switching levels");
        PersistentTerrainSettings.settings.gravityEffect = Random.Range (0.5f, maxGravity);
        print (PersistentTerrainSettings.settings.gravityEffect);
        if (PersistentPlayerSettings.settings == null) {
            // It will be null if we're loading the level from the New Game screen for the
            // first time.
            print ("PersistentPlayerSettings doesn't exist");
            return;
        }
        PersistentPlayerSettings.settings.levelScore = 0;


    }
}
