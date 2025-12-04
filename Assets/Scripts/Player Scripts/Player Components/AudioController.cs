using UnityEngine;

public class PlayerAudio: MonoBehaviour
{
    public static PlayerAudio Instance;

    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Movement / Damage")]
    public AudioClip hurtSFX;
    public AudioClip dashSFX;

    [Header("Shooting")]
    public AudioClip shootSFX;

    [Header("Trace Ability")]
    public AudioClip traceStartSFX;
    public AudioClip traceEndSFX;
    public AudioClip spikeShootSFX;

    [Header("Upgrades / UI")]
    public AudioClip upgradeBuySFX;
    public AudioClip upgradeFailSFX;

    void Awake()
    {
        Instance = this;
    }

    public void Play(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }
}
