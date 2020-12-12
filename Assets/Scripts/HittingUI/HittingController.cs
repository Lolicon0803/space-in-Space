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

    private bool isStop;
    private bool isRunning;

    // Start is called before the first frame update
    void Start()
    {
        audioEngine.ResetAdjustArgs();
        isRunning = false;
        isStop = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStop)
        {
            TouchStates touchState = audioEngine.touchState;
            changeBtnSprite(touchState);
        }
    }

    public void Pause()
    {
        isStop = !isStop;
    }

    public bool isPause()
    {
        return isStop;
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
            // 改為文字
            // audioSource.PlayOneShot(clips[1], 0.03f);
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
        yield return new WaitForSeconds(0.1f);
        isRunning = false;
    }
}
