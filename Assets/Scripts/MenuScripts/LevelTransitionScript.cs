using UnityEngine;
using System.Collections;

public class LevelTransitionScript : MonoBehaviour {
    // Use this for initialization
    //public Texture emptyProgressBar;
    //public Texture fullProgressBar;
    public Font font;

    private AsyncOperation async = null;
    void Start () {
        async = Application.LoadLevelAsync (1);
        Load ();
    }

    IEnumerator Load() {
        yield return async;
    }

    void OnGUI() {
        if (async != null) {
            int width = Camera.main.pixelWidth;
            int height = Camera.main.pixelHeight;
            // Eventually have a loading bar.
            // GUI.DrawTexture(new Rect(width / 2 - 50, height / 2 - 50, 100, 50), emptyProgressBar);
            // GUI.DrawTexture(new Rect(width / 2 - 50, height / 2 - 50, 100 * async.progress, 50), fullProgressBar);
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.skin.font = font;
            GUIStyle myStyle = new GUIStyle(GUI.skin.label);
            myStyle.fontSize = 30;
            GUI.Label(new Rect(width / 2 - 50, height / 2 - 50, 100, 50), string.Format("{0:N0}%", async.progress * 100f), myStyle);
            print(async.progress);
        }
    }
    void OnDestroy() {
        print ("switching levels");
        PersistentPlayerSettings.settings.levelScore = 0;
    }
}
