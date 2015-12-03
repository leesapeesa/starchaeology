using UnityEngine;
using System.Collections;

public class IntroScreenScript : MonoBehaviour {

    public void Play () {
        LevelUtil.EndGame(LevelUtil.GameEndType.NEW_GAME);
    }
}
