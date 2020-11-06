using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapSystem : MonoBehaviour
{
    public int sceneMaxX;
    public int sceneMaxY;

    Dictionary<Vector2, EventBox> Map = new Dictionary<Vector2, EventBox>();


    private static MapSystem singleton;
    public static MapSystem Singleton
    {
        get
        {
            if (singleton == null)
            {
                singleton = FindObjectOfType(typeof(MapSystem)) as MapSystem;
            }
            return singleton;
        }

    }

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
        }

        ClearMapEvent();
    }

    void Start()
    {
        ObjectTempoControl.Singleton.AddToBeatAction(ClearMapEvent, TempoActionType.Whole);
    }

    void Update()
    {

    }


    /**
     * 以下多載，根據傳入資料判斷是哪一種Event
     * 
     */
    public void SetMapEvent(Vector2 coord, EventList type, bool result, Vector2 hit_Diraction,short hit_Distance ,short hit_Power)
    {
        coord = CorrectCoord(coord);


        if (type == EventList.Hit)
        {
            Map[coord].hit_Diraction = hit_Diraction;
            Map[coord].hit_Distance = hit_Distance;
            Map[coord].hit_Power = hit_Power;
        }

        Map[coord].eventBox[type] = result;
    }

    public void SetMapEvent(Vector2 coord, EventList type, bool result)
    {
        coord = CorrectCoord(coord);
        Map[coord].eventBox[type] = result;
    }
    /**
    * 以上多載，根據傳入資料判斷是哪一種Event
    * 
    */


    //清除全地圖事件
    //Fix:請跟節奏系統配合，當節拍抵達時，需先呼叫此事件後才可讓其他物件往地圖更新事件
    void ClearMapEvent()
    {
      
        for (int i = 0; i < sceneMaxX; i++)
        {
            for (int j = 0; j < sceneMaxY; j++)
            {
                Vector2 vec = new Vector2(i, j);
                Map[vec] = new EventBox();
                
            }
        }
    }

    //傳入玩家座標，觸發該座標事件
    public void MapEventTrigger(Vector2 coord)
    {
        coord = CorrectCoord(coord);
        foreach (var mapEvent in Map[coord].eventBox)
        {

            if (mapEvent.Value)
            {
                if (mapEvent.Key == EventList.Die)
                {
                    MapEvent.Singleton.Die();
                }
                else if (mapEvent.Key == EventList.Hit)
                {
                    Debug.Log(Map[new Vector2(0, 2)].eventBox[EventList.Hit]);

                    MapEvent.Singleton.Hit(Map[coord].hit_Diraction, Map[coord].hit_Distance, Map[coord].hit_Power);
                }
                else if (mapEvent.Key == EventList.LostLife)
                {
                    MapEvent.Singleton.LostLife();
                }
            }
        }

    }

    private Vector2 CorrectCoord(Vector2 coord)
    {
        coord.x = (int)Mathf.Floor(coord.x);
        coord.y = (int)Mathf.Floor(coord.y);
        return coord;
    }

}
