using UnityEngine;
using System.Collections;

public class UIAudio : MonoBehaviour {

    private AudioSource audioSource;

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
	}
	
	public void PlayClick()
    {
        audioSource.Play();
    }
}
