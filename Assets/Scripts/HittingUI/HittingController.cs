using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HittingController : MonoBehaviour
{
    public TempoManager audioEngine;
    public Player player;
    public SpriteRenderer hintFrame;
    public GameObject notePrefab;
    public Sprite[] frames;
    public Animator hitAnim;
    public Animator frameAnim;
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

            int recoverCount = player.GetComponent<PlayerLifeSystem>().getRecoverCount();
            changeHintFrame(recoverCount);
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
            if (isReset)
            {
                deleteHitNode();

                generateText("HIT!");

                if (hitAnim != null)
                {
                    hitAnim.SetTrigger("Hit");
                }
            }
            isReset = false;
        }
        else if (touchState == TouchStates.TouchFailed || touchState == TouchStates.TimeOutFailed || touchState == TouchStates.TimeOut)
        {
            if (isReset)
            {
                generateText("MISS!");
            }
            isReset = false;
        }
        else if (touchState == TouchStates.Reset)
        {
            isReset = true;
        }
    }

    void generateText(string str)
    {
        GameObject newText = Instantiate(missPrefab, canvas.transform);
        newText.GetComponent<Text>().text = str;
        newText.transform.position = transform.position;
        newText.transform.Translate(new Vector3(0, 0.9f, 0));
    }

    void deleteHitNode()
    {
        // 最靠近的兩個可能還沒跑完動畫
        if (!gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<NoteObject>().isStop)
        {
            gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<NoteObject>().hit();
            gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<NoteObject>().hit();
        }
        else
        {
            gameObject.transform.GetChild(0).gameObject.transform.GetChild(2).GetComponent<NoteObject>().hit();
            gameObject.transform.GetChild(0).gameObject.transform.GetChild(3).GetComponent<NoteObject>().hit();
        }
    }

    void changeHintFrame(int recoverAfterShoot)
    {
        hintFrame.sprite = frames[recoverAfterShoot];
    }
}
