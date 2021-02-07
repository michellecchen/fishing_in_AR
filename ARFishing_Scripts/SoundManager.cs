using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    AudioSource audio;
    public AudioClip caughtFish;
    public AudioClip[] splashes;

    public AudioClip startFishingPlop;
    
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public void PlaySuccess() {
        audio.clip = caughtFish;
        audio.Play();
    }

    public void PlaySplash() {
        audio.clip = splashes[Random.Range(0, splashes.Length)];
        audio.Play();
    }

    public void PlayPlop() {
        audio.clip = startFishingPlop;
        audio.Play();
    }
}
