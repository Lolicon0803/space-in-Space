using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Linq;
using UnityEngine.Events;

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


public class TempoManager : MonoBehaviour
{
    public SpriteRenderer sprite;
    public bool improveDelayMode;
    public TouchStates touchState;
    public static double delay;
    private double adjustDelay;
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
    private double bufferTime;
    private double lastTouchDelayTime;
    private List<double> touchDelayTimes;
    // 在Beat的多久處Reset
    private double resetStartTime;
    // 在Beat的多久處結束Reset
    private double resetEndTime;
    private double time;
    private float timeStep;
    public TempoActions tempoActions;
    public AudioSource BGM;

    // Start is called before the first frame update
    void Start()
    {
        improveDelayMode = false;
        BPM = 60;
        delay = 0;
        adjustDelay = 0;
        lastTouchDelayTime = 0;
        touchDelayTimes = new List<double>();
        bufferTime = MsPB / 8;
        SetResetArgs(0.4, 0.6);
        BGM.Play();
        time = MsPB;
        timeStep = 0.01f;
        InvokeRepeatInit();
        touchState = TouchStates.Disable;
    }
    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
        }
        tempoActions = new TempoActions();
    }
    void InvokeRepeatInit()
    {
        CancelInvoke();
        InvokeRepeating("Timer", 0, timeStep);
        InvokeRepeating("WholeTimer", 0, (float)(MsPB / 1000));
        InvokeRepeating("HalfTimer", 0, (float)(MsPB / 1000 / 2));
        InvokeRepeating("QuarterTimer", 0, (float)(MsPB / 1000 / 4));
    }
    void Timer()
    {
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
                tempoActions[TempoActionType.TimeOut]();

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
    void WholeTimer()
    {
        tempoActions[TempoActionType.Whole]();
    }
    void HalfTimer()
    {
        tempoActions[TempoActionType.Half]();
    }
    void QuarterTimer()
    {
        tempoActions[TempoActionType.Quarter]();
    }
    // Update is called once per frame
    void Update()
    {
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
    // 打擊更改State
    public bool KeyDown()
    {
        bool touchSuccess = false;
        switch (touchState)
        {
            case TouchStates.Disable:
                touchState = TouchStates.TouchFailed;
                break;
            case TouchStates.Enable:
                touchSuccess = true;
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
        if (improveDelayMode) UpdateAdjustDelay();
        return touchSuccess;
    }
    private void UpdateAdjustDelay()
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
            adjustDelay = newDelay;
        }
    }

    // 設定resetState的起始及結束時間(Beat的0~1)
    private void SetResetArgs(double resetStartTime, double resetEndTime)
    {
        this.resetStartTime = MsPB * resetStartTime;
        this.resetEndTime = MsPB * resetEndTime;
    }
    private bool IsEnableTouchTime()
    {
        double touchTime = ((time - GetDelay()) % MsPB);
        return touchTime <= bufferTime || touchTime >= (MsPB - bufferTime);
    }
    private bool IsResetTime()
    {
        double currentTime = ((time - GetDelay()) % MsPB);
        return currentTime <= resetEndTime && currentTime >= resetStartTime;
    }
    private double GetThisTouchDelayTime()
    {
        double touchTime = (time % MsPB);
        return (touchTime);
    }
    public double GetLastTouchDelayTime()
    {
        return lastTouchDelayTime;
    }
    public double GetCurrentDelayTime()
    {
        return GetDelay();
    }
    public double GetAvgTouchDelayTime()
    {
        if (touchDelayTimes.Count == 0) return 0;
        return touchDelayTimes.Average();
    }
    private double GetDelay()
    {
        return (improveDelayMode) ? adjustDelay : delay;
    }
    public double GetStaticDelay()
    {
        return delay;
    }
    public void BGMReStart()
    {
        BGM.Stop();
        BGM.Play();
        time = MsPB;
        InvokeRepeatInit();
    }
    public void ResetAdjustArgs()
    {
        adjustDelay = 0;
        lastTouchDelayTime = 0;
        touchDelayTimes = new List<double>();
        BGMReStart();
    }
    public void UpdateDelay()
    {
        if (improveDelayMode)
        {
            delay = adjustDelay;
        }
    }
    //單例引用，所有人呼叫的系統都是同一個
    private static TempoManager singleton = null;
    public static TempoManager Singleton
    {
        get
        {
            if (singleton == null)
            {
                singleton = FindObjectOfType(typeof(TempoManager)) as TempoManager;
            }
            return singleton;
        }

    }

}
