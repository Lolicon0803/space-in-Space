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
        audioSource.PlayOneShot(fallIntoBlackHole);
    }

    private void PlayFireBagAudio(Vector2 direction)
    {
        audioSource.PlayOneShot(fireBag);
    }

    private void PlayWalkAudio(Vector2 direction)
    {
        audioSource.PlayOneShot(walk);
    }

    private void PlayErrorAudio()
    {
        audioSource.PlayOneShot(error);
    }

    private void PlayeMissAudio()
    {
        audioSource.PlayOneShot(timeMiss);
    }

}
