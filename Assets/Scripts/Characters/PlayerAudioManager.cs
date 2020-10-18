using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    public AudioClip walk;
    public AudioClip fireBag;
    public AudioClip fallIntoBlackHole;

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



}
