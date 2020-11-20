using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HittingController : MonoBehaviour
{
    public TempoManager audioEngine;
    public Text msg;
    public SpriteRenderer hittingArea;
    public GameObject notePrefab;

    // Start is called before the first frame update
    void Start()
    {
        audioEngine.ResetAdjustArgs();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            audioEngine.KeyDown();
        }

        TouchStates touchState = audioEngine.touchState;
        changeMsg(touchState);
        changeBtnColor(touchState);
    }

    void changeMsg(TouchStates touchState)
    {
        switch (touchState)
        {
            case TouchStates.Reset:
                msg.text = "";
                break;
            case TouchStates.Disable:
                break;
            case TouchStates.Enable:
                break;
            case TouchStates.TimeOut:
                msg.text = "TimeOut";
                break;
            case TouchStates.Touched:
                msg.text = "Success";
                break;
            case TouchStates.TouchFailed:
                msg.text = "Failed";
                break;
            case TouchStates.TimeOutFailed:
                msg.text = "Failed";
                break;
        }
    }

    void changeBtnColor(TouchStates touchState)
    {
        switch (touchState)
        {
            case TouchStates.Reset:
                break;
            case TouchStates.Disable:
                hittingArea.color = new Color(1, 1, 1, 1);
                break;
            case TouchStates.Enable:
                hittingArea.color = new Color(1, 0, 0, 1);
                break;
            case TouchStates.Touched:
                hittingArea.color = new Color(0, 1, 0, 1);
                break;
            case TouchStates.TouchFailed:
                hittingArea.color = new Color(0, 0, 0, 1);
                break;
            case TouchStates.TimeOutFailed:
                hittingArea.color = new Color(1, 1, 1, 1);
                break;
        }
    }
}
