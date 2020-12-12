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
    public Animator hitAnim;
    public Canvas canvas;
    public GameObject missPrefab;

    private bool isStopNow;
    private bool isReset;

    // Start is called before the first frame update
    void Start()
    {
        audioEngine.ResetAdjustArgs();
        isStopNow = false;
        isReset = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStopNow)
        {
            TouchStates touchState = audioEngine.touchState;
            changeBtnSprite(touchState);
        }
    }

    public void Stop()
    {
        isStopNow = true;
    }

    public void Restart()
    {
        isStopNow = false;
    }

    public bool isStop()
    {
        return isStopNow;
    }

    void changeBtnSprite(TouchStates touchState)
    {
        if (touchState == TouchStates.Touched)
        {
            // audioSource.PlayOneShot(clips[0], 0.2f);
            if (hitAnim != null && isReset)
            {
                hitAnim.SetTrigger("Hit");
            }
            isReset = false;
        }
        else if (touchState == TouchStates.TouchFailed || touchState == TouchStates.TimeOutFailed || touchState == TouchStates.TimeOut)
        {
            // audioSource.PlayOneShot(clips[1], 0.03f);
            if (isReset)
            {
                GameObject newText = Instantiate(missPrefab, canvas.transform);
                newText.transform.position = transform.position;
                newText.transform.Translate(new Vector3(0, 0.9f, 0));
            }
            isReset = false;
        }
        else if (touchState == TouchStates.Reset)
        {
            isReset = true;
        }
    }
}
