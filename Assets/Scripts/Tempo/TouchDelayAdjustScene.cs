using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class TouchDelayAdjustScene : MonoBehaviour
{
    public TempoManager audioEngine;
    public Text avgDelayText;
    public Text lastTouchDelayText;
    public Text currentDelayText;
    public Text setDelayText;
    public Text isSuccessText;
    public Button resetButton;
    public Button checkButton;
    // Start is called before the first frame update
    void Start()
    {
        audioEngine.ResetAdjustArgs();
        resetButton.onClick.AddListener(ResetEvent);
        resetButton.onClick.AddListener(selectNull);
        checkButton.onClick.AddListener(audioEngine.UpdateDelay);
        checkButton.onClick.AddListener(selectNull);
    }

    // Update is called once per frame
    void Update()
    {
        // audioEngine.improveDelayMode = true;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            audioEngine.KeyDown();
        }
        avgDelayText.text = "Avg Touch Delay: " + audioEngine.GetAvgTouchDelayTime().ToString();

        lastTouchDelayText.text = "Last Touch Delay: " + audioEngine.GetLastTouchDelayTime().ToString();
        currentDelayText.text = "Current Delay: " + audioEngine.GetCurrentDelayTime().ToString();
        setDelayText.text = "Set Delay: " + audioEngine.GetStaticDelay().ToString();
        TouchStates touchState = audioEngine.touchState;
        switch (touchState)
        {
            case TouchStates.Reset:
                isSuccessText.text = "";
                break;
            case TouchStates.Disable:
                break;
            case TouchStates.Enable:
                break;
            case TouchStates.TimeOut:
                isSuccessText.text = "TimeOut";
                break;
            case TouchStates.Touched:
                isSuccessText.text = "Success";
                break;
            case TouchStates.TouchFailed:
                isSuccessText.text = "Failed";
                break;
            case TouchStates.TimeOutFailed:
                isSuccessText.text = "Failed";
                break;
        }
    }
    void ResetEvent()
    {
        avgDelayText.text = "Avg Touch Delay: 0";
        lastTouchDelayText.text = "Last Touch Delay: 0";
        currentDelayText.text = "Current Delay: 0";
        audioEngine.ResetAdjustArgs();
    }
    void selectNull()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
