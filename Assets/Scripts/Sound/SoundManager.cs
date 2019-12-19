using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //Adding a central place where audio effects can be stored and accessed. Forgoing using an array so that they can be more descriptive in the editor and call easily via scripts

    [Header("HUD Audio Clips")]
    public AudioClip HUDAttackReady; //Referenced in EnergyCooldownTracker.cs
    [Range(0.0f, 1.0f)] public float HUDAttackReadyVolume = 1.0f;
    public AudioClip HUDTeleportAttackReady; //Referenced in EnergyCooldownTracker.cs
    [Range(0.0f, 1.0f)] public float HUDTeleportAttackReadyVolume = 1.0f;

    [Header("Player Audio Clips")]
    public AudioClip PlayerFootsteps; //Referenced in PlayerAudio.cs and used as an animation event
    [Range(0.0f, 1.0f)] public float PlayerFootstepsVolume = 1.0f;
    public AudioClip PlayerJump; //Referenced in PlayerAudio.cs and used as an animation event
    [Range(0.0f, 1.0f)] public float PlayerJumpVolume = 1.0f;
    public AudioClip PlayerLand; //Referenced in PlayerAudio.cs and used as an animation event
    [Range(0.0f, 1.0f)] public float PlayerLandVolume = 1.0f;
    public AudioClip PlayerHurt; //Referenced in PlayerAudio.cs and used as an animation event
    [Range(0.0f, 1.0f)] public float PlayerHurtVolume = 1.0f;
    public AudioClip PlayerDeath; //Referenced in PlayerAudio.cs and used as an animation event
    [Range(0.0f, 1.0f)] public float PlayerDeathVolume = 1.0f;
    public AudioClip PlayerWallSlide; //Referenced in PlayerAudio.cs and used as an animation event 
    [Range(0.0f, 1.0f)] public float PlayerWallSlideVolume = 1.0f;
    public AudioClip PlayerWhipBase; //Referenced in PlayerAudio.cs and used as an animation event
    [Range(0.0f, 1.0f)] public float PlayerWhipBaseVolume = 1.0f;
    public AudioClip PlayerWhipOnHit; //Referenced in PlayerAttackInfo.cs when a successful hit is registered
    [Range(0.0f, 1.0f)] public float PlayerWhipOnHitVolume = 1.0f;
    public AudioClip PlayerThrowBaton; //Referenced in PlayerAudio.cs and used as an animation event
    [Range(0.0f, 1.0f)] public float PlayerThrowBatonVolume = 1.0f;
    public AudioClip PlayerBatonOnHit; //Referenced in Baton_Projectile.cs when a successful hit is registered
    [Range(0.0f, 1.0f)] public float PlayerBatonOnHitVolume = 1.0f;
    public AudioClip PlayerTeleportOut; //Referenced in PlayerAudio.cs and used as an animation event
    [Range(0.0f, 1.0f)] public float PlayerTeleportOutVolume = 1.0f;
    public AudioClip PlayerTeleportIn; //Referenced in PlayerAudio.cs and used as an animation event
    [Range(0.0f, 1.0f)] public float PlayerTeleportInVolume = 1.0f;

    [Header("Splicer Audio Clips")]
    public AudioClip SplicerHurt; //Referenced in SplicerAudio.cs and used as an animation event
    [Range(0.0f, 1.0f)] public float SplicerHurtVolume = 1.0f;
    public AudioClip SplicerDeath; //Referenced in SplicerAudio.cs and used as an animation event
    [Range(0.0f, 1.0f)] public float SplicerDeathVolume = 1.0f;
    public AudioClip SplicerAttack; //Referenced in SplicerAudio.cs and used as an animation event
    [Range(0.0f, 1.0f)] public float SplicerAttackVolume = 1.0f;

    [Header("Drone Audio Clips")]
    public AudioClip DroneHurt; //Referenced in DroneAudio.cs and used as an animation event
    [Range(0.0f, 1.0f)] public float DroneHurtVolume = 1.0f;
    public AudioClip DroneDeath;//Referenced in DroneAudio.cs and used as an animation event
    [Range(0.0f, 1.0f)] public float DroneDeathVolume = 1.0f;
    public AudioClip DroneFireProjectile; //Referenced in DroneAudio.cs and used as an animation event
    [Range(0.0f, 1.0f)] public float DroneFireProjectileVolume = 1.0f;
    public AudioClip DroneProjectileOnHit; //Referenced in Projectile.cs when a successful hit is registered
    [Range(0.0f, 1.0f)] public float DroneProjectileOnHitVolume = 1.0f;

    [Header("Boss Audio Clips")]
    public AudioClip BossHurt; //Referenced in BossAudio.cs and used as an animation event
    [Range(0.0f, 1.0f)] public float BossHurtVolume = 1.0f;
    public AudioClip BossDeath; //Referenced in BossAudio.cs and used as an animation event
    [Range(0.0f, 1.0f)] public float BossDeathVolume = 1.0f;
    public AudioClip BossDeathImpact; //Referenced in BossAudio.cs and used as an animation event
    [Range(0.0f, 1.0f)] public float BossDeathImpactVolume = 1.0f;
    public AudioClip BossHammerAttack; //Referenced in BossAudio.cs and used as an animation event
    [Range(0.0f, 1.0f)] public float BossHammerAttackVolume = 1.0f;
    public AudioClip BossFootsteps; //Referenced in BossAudio.cs and used as an animation event
    [Range(0.0f, 1.0f)] public float BossFootstepskVolume = 1.0f;

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
    /// Play a single clip through the sound effects source
    /// </summary>
    /// <param name="clip"></param>
    public void Play(AudioSource source, AudioClip clip)
    {
        source.pitch = 1.0f;
        source.clip = clip;
        source.Play();
    }

    /// <summary>
    /// Stop playback a single clip through the sound effects source
    /// </summary>
    /// <param name="clip"></param>
    public void Stop(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
        source.Stop();
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
