using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //Adding a central place where audio effects can be stored and accessed. Forgoing using an array so that they can be more descriptive in the editor and call easily via scripts
    [Header("Player Audio Clips")]
    public AudioClip PlayerFootsteps; //Referenced in PlayerAudio and used as an animation event
    public AudioClip PlayerJump; //Referenced in PlayerAudio and used as an animation event
    public AudioClip PlayerLand; //Referenced in PlayerAudio and used as an animation event
    public AudioClip PlayerHurt; //Referenced in PlayerAudio and used as an animation event
    public AudioClip PlayerDeath; //Referenced in PlayerAudio and used as an animation event
    public AudioClip PlayerWallSlide; //Referenced in PlayerAudio and used as an animation event 
    public AudioClip PlayerWhipBase; //Referenced in PlayerAudio and used as an animation event
    public AudioClip PlayerWhipOnHit; //Referenced in PlayerAttackInfo.cs when a successful hit is registered
    public AudioClip PlayerThrowBaton; //Referenced in PlayerAudio and used as an animation event
    public AudioClip PlayerBatonOnHit; //Referenced in Baton_Projectile.cs when a successful hit is registered
    public AudioClip PlayerTeleportOut; //Referenced in PlayerAudio and used as an animation event
    public AudioClip PlayerTeleportIn; //Referenced in PlayerAudio and used as an animation event

    [Header("Splicer Audio Clips")]
    public AudioClip SplicerHurt; //Referenced in SplicerAudio and used as an animation event
    public AudioClip SplicerDeath; //Referenced in SplicerAudio and used as an animation event
    public AudioClip SplicerAttack; //Referenced in SplicerAudio and used as an animation event

    [Header("Drone Audio Clips")]
    public AudioClip DroneHurt; //Referenced in DroneAudio and used as an animation event
    public AudioClip DroneDeath;//Referenced in DroneAudio and used as an animation event
    public AudioClip DroneFireProjectile; //Referenced in DroneAudio and used as an animation event
    public AudioClip DroneProjectileOnHit; //Referenced in Projectile.cs when a successful hit is registered

    //Random pitch range
    [Header("Pitch Variance")]
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
    public void Play(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }

    public void PlayWithRandomizedPitch(AudioSource source, AudioClip clip)
    {
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        source.pitch = randomPitch;
        source.clip = clip;
        source.Play();
    }

    /// <summary>
    /// Play a single clip through the music source
    /// </summary>
    /// <param name="clip"></param>
    public void PlayMusic(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }

    /// <summary>
    /// Play a random clip from an array and randomize the pitch slightly
    /// </summary>
    /// <param name="clips"></param>
    public void RandomizeSoundEffect(AudioSource source, params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        source.pitch = randomPitch;
        source.clip = clips[randomIndex];
        source.PlayOneShot(source.clip);
    }
}
