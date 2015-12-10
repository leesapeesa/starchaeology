using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SetVolume : MonoBehaviour {

    public List<AudioClip> audioSources;
    private AudioSource source;

    public static SetVolume instance = null;
    public static SetVolume Instance {
        get { return instance; }
    }

    public void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        } else {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public void changeMusic() {
        source = GameObject.FindGameObjectWithTag("Volume").GetComponent<AudioSource>();

        int choices = audioSources.Count;

        float choice = Random.Range(0, choices);
        int index = (int)choice;
        AudioClip audioChoice = audioSources[index];

        if (PlayerPrefs.HasKey("MusicVolume")) {
            source.volume = PlayerPrefs.GetFloat("MusicVolume");
        }

        source.clip = audioChoice;
        source.loop = true;
        source.Play();
    }

    // Use this for initialization
    public void Start() {
        if (PlayerPrefs.HasKey("Volume")) {
            AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        }
        changeMusic();
    }
    
    // Update is called once per frame
    void Update () {
	
	}
}
