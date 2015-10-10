using UnityEngine;
using System.Collections;

public abstract class Objective : MonoBehaviour {

    public abstract bool ObjectiveComplete();
    public abstract bool ObjectiveFailed();

}
