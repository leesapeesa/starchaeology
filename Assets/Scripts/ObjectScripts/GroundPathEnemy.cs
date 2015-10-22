using UnityEngine;
using System.Collections;

/// <summary>
/// PathEnemy controls an enemy which always moves along a predefined path along the ground.
/// </summary>
public class GroundPathEnemy : Enemy {

    private float[] waypoints; // An ordered list of points that this enemy must visit
    private int numWaypoints = 2; // Number of waypoints this enemy should use, measured as x-coordinates. Keeping it at 2 for now to stay simple.
    private int waypointIndex = 0; // The current waypoint the enemy is moving towards
    private Rigidbody2D rigidbody2d;
    [SerializeField] private float velocity = 1;
    private const float THRESHOLD = 0.5f;

	// Use this for initialization
	void Start () {
        rigidbody2d = GetComponent<Rigidbody2D>();

        //Right now we are having the enemy walk back and forth along a path of random length
        float curX = rigidbody2d.position.x;
        float pathLength = Random.Range(10f, 30f);
        waypoints = new float[] { Mathf.Clamp(curX - pathLength / 2, minX, maxX), Mathf.Clamp(curX + pathLength / 2, minX, maxX) };
	}
	
	void FixedUpdate () {
        float curX = rigidbody2d.position.x;
        float distanceToWaypoint = curX - waypoints[waypointIndex];

        //update the enemy's movement
        rigidbody2d.velocity = new Vector2(Mathf.Sign(distanceToWaypoint) * -velocity, rigidbody2d.velocity.y);

        //if we have reached the waypoint, move on to the next one.
        if (Mathf.Abs(distanceToWaypoint) <= THRESHOLD)
            waypointIndex = (waypointIndex + 1) % numWaypoints;
	}
}
