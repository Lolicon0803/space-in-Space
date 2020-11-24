using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBase : MonoBehaviour
{

    public Dictionary<string, bool> readStories;

    private static DataBase singleton;
    public static DataBase Singleton
    {
        get
        {
            if (singleton == null)
            {
                singleton = FindObjectOfType(typeof(DataBase)) as DataBase;
            }
            return singleton;
        }

    }
    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            readStories = new Dictionary<string, bool>();
            DontDestroyOnLoad(gameObject);
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
        }
    }



}
