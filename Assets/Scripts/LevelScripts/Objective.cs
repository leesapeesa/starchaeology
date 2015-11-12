using UnityEngine;
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

    public abstract bool ObjectiveComplete();
    public abstract bool ObjectiveFailed();

}
