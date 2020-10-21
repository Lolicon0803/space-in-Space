using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAudioManager : MonoBehaviour
{
    public AudioClip walk;
    public AudioClip fireBag;
    public AudioClip fallIntoBlackHole;
    public AudioClip timeMiss;
    public AudioClip error;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        playerMovement.OnWalk += PlayWalkAudio;
        playerMovement.OnFireBag += PlayFireBagAudio;
        playerMovement.OnFallIntoBlackHole += PlayFallIntoBlackHoleAudio;
        playerMovement.OnMiss += PlayeMissAudio;
        playerMovement.OnError += PlayErrorAudio;
    }

    private void PlayFallIntoBlackHoleAudio()
    {
        if (fallIntoBlackHole != null)
            audioSource.PlayOneShot(fallIntoBlackHole);
    }

    private void PlayFireBagAudio(Vector2 direction)
    {
        if (fireBag != null)
            audioSource.PlayOneShot(fireBag);
    }

    private void PlayWalkAudio(Vector2 direction)
    {
        if (walk != null)
            audioSource.PlayOneShot(walk);
    }

    private void PlayErrorAudio()
    {
        if (error != null)
            audioSource.PlayOneShot(error);
    }

    private void PlayeMissAudio()
    {
        if (timeMiss != null)
            audioSource.PlayOneShot(timeMiss);
    }
}
