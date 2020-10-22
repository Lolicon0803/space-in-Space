using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectTempoControl : MonoBehaviour
{
    private UnityAction QuarterAction = new UnityAction(() => { });
    private UnityAction HalfAction = new UnityAction(() => { });
    private UnityAction WholeAction = new UnityAction(() => { });

    private static ObjectTempoControl singleton = null;
    public static ObjectTempoControl Singleton
    {
        get
        {
            if (singleton == null)
            {
                singleton = FindObjectOfType(typeof(ObjectTempoControl)) as ObjectTempoControl;
            }
            return singleton;
        }

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

        Debug.Log("送給節奏系統");
        // AudioEngine.Singleton.SetTempoTypeListener(QuarterAction, TempoActionType.Quarter);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddToBeatAction(UnityAction function, GameData.TempoType type)
    {
        if (type == GameData.TempoType.Whole)
        {
            WholeAction += function;
            AudioEngine.Singleton.SetTempoTypeListener(WholeAction, TempoActionType.Whole);
        }
        else if (type == GameData.TempoType.Half)
        {
            HalfAction += function;
            AudioEngine.Singleton.SetTempoTypeListener(HalfAction, TempoActionType.Half);
        }
        else if (type == GameData.TempoType.Quarter)
        {
            QuarterAction += function;
            AudioEngine.Singleton.SetTempoTypeListener(QuarterAction, TempoActionType.Quarter);
        }


    }

    public void RemoveToBeatAction(UnityAction function, GameData.TempoType type)
    {
        if (type == GameData.TempoType.Whole)
        {
            WholeAction -= function;
            AudioEngine.Singleton.SetTempoTypeListener(WholeAction, TempoActionType.Whole);
        }
        else if (type == GameData.TempoType.Half)
        {
            HalfAction -= function;
            AudioEngine.Singleton.SetTempoTypeListener(HalfAction, TempoActionType.Half);
        }
        else if (type == GameData.TempoType.Quarter)
        {
            QuarterAction -= function;
            AudioEngine.Singleton.SetTempoTypeListener(QuarterAction, TempoActionType.Quarter);
        }


    }

}
