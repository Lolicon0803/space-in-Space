using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Linq;

public enum TouchStates
{
    Touched,
    TouchFailed,
    TimeOut,
    TimeOutFailed,
    Disable,
    Enable,
    Reset
}
public class AudioEngine: MonoBehaviour
{
    SpriteRenderer sprite;
    public bool improveDelayMode;
    public TouchStates touchState;
    private double bpm;
    public double BPM
    {
        get => bpm;
        set
        {
            bpm = value;
            MsPB = 1000.0 / (this.bpm / 60.0);
        }
    }
    // mini second per beat 
    private double MsPB;
    private double delay;
    private double bufferTime;
    private double lastTouchDelayTime;
    private List<double> touchDelayTimes;
    // 在Beat的多久處Reset
    private double resetStartTime;
    // 在Beat的多久處結束Reset
    private double resetEndTime;
    private double time;
    private float timeStep;
    AudioSource BGM;
 
    // Start is called before the first frame update
    void Start()
    {
        improveDelayMode = false;
        sprite = GetComponent<SpriteRenderer>();
        BPM = 120;
        delay = 0;
        lastTouchDelayTime = 0;
        touchDelayTimes = new List<double>();
        bufferTime = MsPB / 8;
        SetResetArgs(0.4, 0.6);
        BGM = this.GetComponentInChildren<AudioSource>();
        BGM.Play();
        time = MsPB;
        timeStep = 0.01f;
        InvokeRepeating("timer", timeStep, timeStep);
        touchState = TouchStates.Disable;
    }
    void timer() {
        this.time += 0.01 * 1000;
        #region 時間更改State
        if (IsEnableTouchTime())
        {
            // 進入可以打擊時段
            if (touchState == TouchStates.Disable)
            {
                touchState = TouchStates.Enable;
            }
        }
        else
        {
            // Timeout
            if (touchState == TouchStates.Enable)
            {
                touchState = TouchStates.TimeOut;
            }
            // reset結束
            if (touchState == TouchStates.Reset && !IsResetTime())
            {
                touchState = TouchStates.Disable;
            }
        }
        if (IsResetTime())
        {
            touchState = TouchStates.Reset;
        }
        #endregion
    }
    // Update is called once per frame
    void Update()
    {
        #region 打擊更改State
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (touchState)
            {
                case TouchStates.Disable:
                        touchState = TouchStates.TouchFailed;
                    break;
                case TouchStates.Enable:
                        touchState = TouchStates.Touched;
                    break;
                case TouchStates.TimeOut:
                        touchState = TouchStates.TimeOutFailed;
                    break;
                case TouchStates.Reset:
                    break;
                case TouchStates.Touched:
                    break;
                case TouchStates.TouchFailed:
                    break;
                case TouchStates.TimeOutFailed:
                    break;
            }
            Touch();
        }


        #endregion
        #region 狀態處理
        switch (touchState)
        {
            case TouchStates.Reset:
                break;
            case TouchStates.Disable:
                sprite.color = new Color(1, 1, 1, 1);
                break;
            case TouchStates.Enable:
                sprite.color = new Color(1, 0, 0, 1);
                break;
            case TouchStates.Touched:
                sprite.color = new Color(0, 1, 0, 1);
                break;
            case TouchStates.TouchFailed:
                sprite.color = new Color(0, 0, 0, 1);
                break;
            case TouchStates.TimeOutFailed:
                sprite.color = new Color(1, 1, 1, 1);
                break;
        }

        #endregion
    }
    private void Touch()
    {
        lastTouchDelayTime = GetThisTouchDelayTime();
        touchDelayTimes.Add(lastTouchDelayTime);
        if (touchDelayTimes.Count > 10)
        {
            touchDelayTimes.RemoveAt(0);
        }
        if (improveDelayMode && touchDelayTimes.Count == 10)
        {
            double newDelay = GetAvgTouchDelayTime();
            Debug.Log(newDelay);
            delay = newDelay;
        }
    }

    // 設定resetState的起始及結束時間(Beat的0~1)
    private void SetResetArgs(double resetStartTime, double resetEndTime)
    {
        this.resetStartTime = MsPB * resetStartTime;
        this.resetEndTime = MsPB * resetEndTime;
    }
    private bool IsEnableTouchTime() {
        double touchTime = ((time - delay) % MsPB) ;
        return touchTime <= bufferTime || touchTime >= (MsPB - bufferTime);
    }
    private bool IsResetTime()
    {
        double currentTime = ((time - delay) % MsPB);
        return currentTime <= resetEndTime && currentTime >= resetStartTime;
    }
    private double GetThisTouchDelayTime() {
        double touchTime = (time  % MsPB) ;
        return (touchTime);
    }
    public double GetLastTouchDelayTime() {
        return lastTouchDelayTime;
    }
    public double GetCurrentDelayTime()
    {
        return delay;
    }
    public double GetAvgTouchDelayTime()
    {
        if (touchDelayTimes.Count == 0) return 0;
        return touchDelayTimes.Average();
    }
}
