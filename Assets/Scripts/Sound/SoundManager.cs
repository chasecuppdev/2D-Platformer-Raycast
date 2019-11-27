using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //Audio Components
    public AudioSource effectsSource; //Hooked up in editor
    public AudioSource musicSource; //Hooked up in editor

    //Random pitch range
    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;

    //Singleton instance
    public static SoundManager Instance = null;

    //Initialize singleton
    private void Awake()
    {
        // If there is not already an instance of SoundManager, set it to this.
        if (Instance == null)
        {
            Instance = this;
        }
        //If an instance already exists, destroy whatever this object is to enforce the singleton.
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Play a singe clip through the sound effects source
    /// </summary>
    /// <param name="clip"></param>
    public void Play(AudioClip clip)
    {
        effectsSource.clip = clip;
        effectsSource.Play();
    }

    public void PlayWithRandomizedPitch(AudioClip clip)
    {
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        effectsSource.pitch = randomPitch;
        effectsSource.clip = clip;
        effectsSource.Play();
    }

    /// <summary>
    /// Play a single clip through the music source
    /// </summary>
    /// <param name="clip"></param>
    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    /// <summary>
    /// Play a random clip from an array and randomize the pitch slightly
    /// </summary>
    /// <param name="clips"></param>
    public void RandomizeSoundEffect(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        effectsSource.pitch = randomPitch;
        effectsSource.clip = clips[randomIndex];
        effectsSource.Play();
    }
}
