using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectTempoControl : MonoBehaviour
{
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
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddToBeatAction(UnityAction function, TempoActionType type)
    {
        TempoManager.Singleton.tempoActions[type] += function;
    }

    public void RemoveToBeatAction(UnityAction function, TempoActionType type)
    {
        TempoManager.Singleton.tempoActions[type] -= function;
    }

    public void ClearToBeatAction(TempoActionType type)
    {
        //Fix me:not sure if sentence true
        TempoManager.Singleton.tempoActions[type] = null;
    }
}
