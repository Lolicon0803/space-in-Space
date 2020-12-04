using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HittingController : MonoBehaviour
{
    public TempoManager audioEngine;
    public SpriteRenderer hittingArea;
    public GameObject notePrefab;
    public AudioSource audioSource;
    public AudioClip[] clips;
    public Sprite[] sprites;
    private bool isRunning;

    // Start is called before the first frame update
    void Start()
    {
        audioEngine.ResetAdjustArgs();
        isRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            audioEngine.KeyDown();
        }

        TouchStates touchState = audioEngine.touchState;
        changeBtnSprite(touchState);
    }

    void changeBtnSprite(TouchStates touchState)
    {
        if (touchState == TouchStates.Touched && !isRunning)
        {
            audioSource.PlayOneShot(clips[0], 0.2f);
            StartCoroutine(playHitAnim());
        }
        else if (touchState == TouchStates.TouchFailed || touchState == TouchStates.TimeOutFailed || touchState == TouchStates.TimeOut)
        {
            audioSource.PlayOneShot(clips[1], 0.03f);
        }
    }

    IEnumerator playHitAnim()
    {
        isRunning = true;
        hittingArea.sprite = sprites[0];
        yield return new WaitForSeconds(0.08f);
        hittingArea.sprite = sprites[2];
        yield return new WaitForSeconds(0.08f);
        hittingArea.sprite = sprites[1];
        yield return new WaitForSeconds(0.08f);
        hittingArea.sprite = sprites[0];
        yield return new WaitForSeconds(0.5f);
        isRunning = false;
    }
}
