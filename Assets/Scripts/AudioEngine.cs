using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

enum TouchStates
{
    Touched,
    Disable,
    Enable
}
public class AudioEngine: MonoBehaviour
{
    SpriteRenderer sprite;

    private double BPM;
    // mini second per beat 
    private double MsPB;
    private double delay;
    private double bufferTime;
    private double time;
    private TouchStates touchState;
    AudioSource BGM;
    public void setBPM(int newBPM) {
        BPM = newBPM;
        MsPB = 1000.0 / (this.BPM / 60.0);
    }
    public void setBGM() {

    }
 
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        setBPM(120);
        delay = -200;
        bufferTime = MsPB / 8;
        BGM = this.GetComponentInChildren<AudioSource>();
        BGM.Play();
        time = 0;
    }
    // Update is called once per frame
    void Update()
    {
        this.time += Time.deltaTime;
        //Debug.Log((time*1000).ToString()+"," +getMsPB().ToString());
        if (isEnableTouchTime())
        {
            if (touchState == TouchStates.Disable)
            {
                touchState = TouchStates.Enable;
            }
        }
        else
        {
            touchState = TouchStates.Disable;
        }


        switch (touchState)
        {
            case TouchStates.Disable:
                sprite.color = new Color(1, 1, 1, 1);
                break;
            case TouchStates.Enable:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    touchState = TouchStates.Touched;
                    Debug.Log(getThisTouchDelayTime());
                }
                sprite.color = new Color(1, 0, 0, 1);
                break;
            case TouchStates.Touched:
                sprite.color = new Color(0, 1, 0, 1);
                //Debug.Log("Perfect");
                break;
        }

    }

    bool isEnableTouchTime() {
        double touchTime = (time * 1000 % MsPB) - delay;
        return touchTime <= bufferTime || touchTime >= (MsPB - bufferTime);
    }
    double getThisTouchDelayTime() {
        double touchTime = (time * 1000 % MsPB) - delay;
        if (touchTime <= bufferTime)
        {
            return (touchTime);
        }
        else
        {
            return (touchTime - MsPB);
        }
    }
}
