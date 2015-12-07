using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class Objective {

    protected const string RETURN_TO_SHIP = "Get back to your ship!";

    /// <summary>
    /// How many Special Items this objective needs to spawn
    /// </summary>
    public virtual int NumSpecialItems { get { return 0; } }
    /// <summary>
    /// How many point collectibles this objective needs to spawn
    /// </summary>
    public virtual int NumPointItems { get { return 0; } }
    /// <summary>
    /// What kind of collectible type, if any, this objective requires
    /// </summary>
    public virtual int? GoalCollectibleType { get { return null; } }
    /// <summary>
    /// How should enemies behave after the player has finished this objective and is trying to get back to the ship?
    /// </summary>
    public abstract GroundPathEnemy.Behavior EnemyBehaviorOnComplete { get; }
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
