using UnityEngine;
using System.Collections;

/// <summary>
/// A version of GroundPathEnemy that kills you upon contact
/// </summary>
public class DeathPathEnemy : GroundPathEnemy {

    void OnCollisionEnter2D(Collision2D coll)
    {
        // kill player upon contact
        if (coll.gameObject.tag == "Player") {
            coll.gameObject.GetComponent<PlayerCharacter2D>().health = 0;
            audioSource.Play();
        }
    }
}
