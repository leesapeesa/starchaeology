using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour {
	
	public float moveTime;
	public LayerMask blockingLayer;
	
	private BoxCollider2D boxCollider;
	private Rigidbody2D	rb2D;
	private float inverseMoveTime;
	// Use this for initialization
	protected virtual void Start () {
		boxCollider = GetComponent<BoxCollider2D> ();
		rb2D = GetComponent<Rigidbody2D> ();
		inverseMoveTime = 1f / moveTime;
	}
	
	protected bool Move (int xDir, int yDir, out RaycastHit2D hit) {
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2 (xDir, yDir);
		
		boxCollider.enabled = false;
		
		hit = Physics2D.Linecast (start, end, blockingLayer);
		boxCollider.enabled = true;
		
		if (hit.transform == null) {
			print ("supposed to move???");
			StartCoroutine(SmoothMovement (end));
			return true;
		} else {
			return false;
		}
	}
	
	protected virtual void AttemptMove <T> (int xDir, int yDir)
		where T : Component
	{
		print ("In Moving Object AttemptMove");
		RaycastHit2D hit;
		bool canMove = Move (xDir, yDir, out hit);
		
		if (hit.transform == null)
			return;
		
		T hitComponent = hit.transform.GetComponent<T> ();
		
		print ("Can Move? " + canMove);
		print ("Hit? " + hitComponent != null);
		if (!canMove && hitComponent != null) {
			print ("Can't move");
			OnCantMove (hitComponent);
		}
	}
	protected IEnumerator SmoothMovement (Vector3 end) {
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
		print ("sqrRemainingDistance: " + sqrRemainingDistance);
		while (sqrRemainingDistance > float.Epsilon) {
			Vector3 newPosition = Vector3.MoveTowards (rb2D.position, end, inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition (newPosition);
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			print ("transform.position " + transform.position);
			print ("moving to: " + newPosition);
			print ("sqrRemainingDistance in Loop; " + sqrRemainingDistance);
			yield return null;
		}
	}
	
	protected abstract void OnCantMove <T> (T component)
		where T : Component;
	
}

