using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum EventList
{
    LostLife,
    Hit,
    Die,
}

class EventBox
{
    public Vector2 hit_Diraction;
    public short hit_Distance;
    public short hit_Power;

    public Dictionary<EventList, bool> eventBox = new Dictionary<EventList, bool>()
    {
        [EventList.LostLife] = false,
        [EventList.Hit] = false,
        [EventList.Die] = false,
    };
}


public class MapEvent : MonoBehaviour
{
    private static MapEvent singleton;
    public static MapEvent Singleton
    {
        get
        {
            if (singleton == null)
            {
                singleton = FindObjectOfType(typeof(MapEvent)) as MapEvent;
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

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Die()
    {
        Player.Singleton.lifeSystem.GameOver();
    }

    public void LostLife()
    {
        Player.Singleton.lifeSystem.Hurt();
    }

    public void Hit(Vector2 hit_Diraction, short hit_Distance, short hit_Power)
    {
        Debug.Log("撞擊");
        Player.Singleton.movement.Knock(hit_Diraction, hit_Distance, hit_Power);
    }
}
