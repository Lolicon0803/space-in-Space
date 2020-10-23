using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public enum TempoActionType
{
    Quarter,
    Half,
    Whole,
    TimeOut,
}
public class TempoActions
{
    private Dictionary<TempoActionType, UnityAction> tempoActionDictionary;
    public TempoActions()
    {
        tempoActionDictionary = new Dictionary<TempoActionType, UnityAction>();
        foreach (TempoActionType type in Enum.GetValues(typeof(TempoActionType)))
        {
            tempoActionDictionary.Add(type, () => { //Debug.Log(type.ToString());
             });
        }
    }
    public UnityAction this[TempoActionType key]
    {
        get => tempoActionDictionary[key];
        set => tempoActionDictionary[key] = value;
    }
}

