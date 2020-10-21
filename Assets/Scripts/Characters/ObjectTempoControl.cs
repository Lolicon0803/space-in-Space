using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectTempoControl : MonoBehaviour
{
    private UnityAction QuarterAction = new UnityAction(() => { });
    private UnityAction HalfAction;
    private UnityAction WholeAction;

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

    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("送給節奏系統");
        AudioEngine.Singleton.SetTempoTypeListener(QuarterAction, TempoActionType.Whole);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddToBeatAction(UnityAction function)
    {
        QuarterAction += function;
        AudioEngine.Singleton.SetTempoTypeListener(QuarterAction, TempoActionType.Whole);
    }


}
