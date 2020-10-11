using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TouchDelayAdjustScene : MonoBehaviour
{
    AudioEngine audioEngine;
    Text avgDelayText;
    Text lastTouchDelayText;
    Text currentDelayText;
    Text isSuccessText;
    // Start is called before the first frame update
    void Start()
    {
        audioEngine = this.GetComponentInChildren<AudioEngine>();
        avgDelayText = transform.Find("Canvas/AvgDelayText").GetComponent<Text>();
        lastTouchDelayText = transform.Find("Canvas/LastTouchDelayText").GetComponent<Text>();
        currentDelayText = transform.Find("Canvas/CurrentDelayText").GetComponent<Text>();
        isSuccessText = transform.Find("Canvas/IsSuccessText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        avgDelayText.text = "Avg Touch Delay: " + audioEngine.GetAvgTouchDelayTime().ToString();
        lastTouchDelayText.text = "Last Touch Delay: " + audioEngine.GetLastTouchDelayTime().ToString();
        currentDelayText.text = "Current Delay: " + audioEngine.GetCurrentDelayTime().ToString();
        TouchStates touchState = audioEngine.touchState;
        audioEngine.improveDelayMode = true;
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
}
