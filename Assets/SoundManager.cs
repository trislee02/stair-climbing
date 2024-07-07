using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource backgroundAudioSource;
    [SerializeField]
    private AudioSource soundEffectAudioSource;

    [Header("Sound Effects")]
    [SerializeField]
    private AudioClip backgroundMusic;
    [SerializeField]
    private AudioClip stepSound;
    [SerializeField]
    private AudioClip throwSound;
    [SerializeField]
    private AudioClip hitSound;
    [SerializeField]
    private AudioClip collectSound;
    [SerializeField]
    private AudioClip gameoverSound;
    [SerializeField]
    private AudioClip levelUpSound;

    // Start is called before the first frame update
    void Start()
    {
        PlayBackgroundMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayBackgroundMusic()
    {
        backgroundAudioSource.clip = backgroundMusic;
        backgroundAudioSource.loop = true;
        backgroundAudioSource.volume = 0.5f;
        backgroundAudioSource.Play();
    }

    public void PlayStepSound()
    {
        soundEffectAudioSource.PlayOneShot(stepSound);
    }

    public void PlayThrowSound()
    {
        soundEffectAudioSource.PlayOneShot(throwSound);
    }

    public void PlayCollectSound()
    {
        soundEffectAudioSource.PlayOneShot(collectSound);
    }

    public void PlayHitSound()
    {
        soundEffectAudioSource.PlayOneShot(hitSound);
    }

    public void PlayGameOverSound()
    {
        soundEffectAudioSource.PlayOneShot(gameoverSound);
    }

    public void PlayLevelUpSound()
    {
        soundEffectAudioSource.PlayOneShot(levelUpSound);
    }
}
