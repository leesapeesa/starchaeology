using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class Objective {

    /// <summary>
    /// How many Special Items this objective needs to spawn
    /// </summary>
    public virtual int NumSpecialItems { get { return 0; } }
    /// <summary>
    /// How many point collectibles this objective needs to spawn
    /// </summary>
    public virtual int NumPointItems { get { return 0; } }
    /// <summary>
    /// Textual representation of the specific objective type, used to identify objectives when saving games.
    /// </summary>
    public abstract string Type { get; }

    public abstract bool ObjectiveComplete();
    public abstract bool ObjectiveFailed();

    public Objective()
    {
        //by default, disable the timer UI
        //objectives that need this will have to enable it themselves.
        GameObject.Find("TimerBar").GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);

    }

}
