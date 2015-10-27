using UnityEngine;
using System.Collections;

/// <summary>
/// PathEnemy controls an enemy which always moves along a predefined path along the ground.
/// </summary>
public class GroundPathEnemy : Enemy {

    private float[] waypoints; // An ordered list of points that this enemy must visit, measured as x-coordinates
    private int numWaypoints = 2; // Number of waypoints this enemy should use. Keeping it at 2 for now to stay simple.
    private int waypointIndex = 0; // The current waypoint the enemy is moving towards
    private Rigidbody2D rigidbody2d;
    [SerializeField] private float velocity = 1;
    protected AudioSource audioSource;
    protected const float THRESHOLD = 0.5f;
    protected const float DAMAGE = 20;

	// Use this for initialization
	void Start () {
        rigidbody2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

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

    void OnCollisionEnter2D(Collision2D coll)
    {
        // if we are colliding with the player, hurt the player
        if (coll.gameObject.tag == "Player") {
            coll.gameObject.GetComponent<PlayerCharacter2D>().health -= DAMAGE;
            audioSource.Play();
        }
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        //If there is an object blocking our way to the next waypoint, "bounce" off it and go to the next one
        if (coll.gameObject.tag != "TerrainCollider") {
            float distanceToWaypoint = rigidbody2d.position.x - waypoints[waypointIndex];
            float objDistToWaypoint = coll.gameObject.GetComponent<Rigidbody2D>().position.x - waypoints[waypointIndex];
            if (Mathf.Abs(objDistToWaypoint) < Mathf.Abs(distanceToWaypoint)) {
                waypointIndex = (waypointIndex + 1) % numWaypoints;
            }
        }
    }
}
