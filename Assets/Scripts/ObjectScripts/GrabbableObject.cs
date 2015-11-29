using UnityEngine;
using System.Collections;

/// <summary>
/// Represents a Non Player Object that can be grabbed by the player
/// </summary>
public class GrabbableObject : NonPlayerObject {

    [SerializeField] private float width;

    /// <summary>
    /// Call this function when you want to grab this object
    /// </summary>
	public void OnGrab(GameObject grabber, float xOffset, float yOffset)
    {
        //parent to the grabber's transform so that we follow it
        transform.SetParent(grabber.transform);
        //we need to disable rigidbody effects, because Unity gets unhappy
        //when rigidbodies get parented.
        GetComponent<Rigidbody2D>().isKinematic = true;
        transform.rotation = Quaternion.identity;
        transform.localPosition = new Vector2(xOffset + width / 2, yOffset);
        transform.localScale = new Vector2(1 / grabber.transform.localScale.x, 1 / grabber.transform.localScale.y);
    }

    /// <summary>
    /// Returns this object to normal behavior
    /// </summary>
    public void EndGrab(GameObject grabber, float xOffset, float yOffset)
    {
        //make sure all transforms are set correctly, to deal with the case where the grabber has changed
        //direction since picking up this object.
        transform.rotation = Quaternion.identity;
        transform.localPosition = new Vector2(xOffset + width / 2, yOffset);
        transform.localScale = new Vector2(1 / grabber.transform.localScale.x, 1 / grabber.transform.localScale.y);

        GetComponent<Rigidbody2D>().isKinematic = false;
        transform.SetParent(null);
        transform.localScale = Vector3.one;
    }
}
