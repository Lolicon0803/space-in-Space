using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public enum GroundBehavior
{
    [Tooltip("預設，撞到後回到格子上")]
    None,
    [Tooltip("可以站的")]
    Standable,
    [Tooltip("可以彈的")]
    Rebounce
}

[System.Serializable]
public class GroundEvent
{
    [Tooltip("從哪個方向來。")]
    public Direction inDirection;
    [Tooltip("從inDirection來時要有的行為。")]
    public GroundBehavior behavior;
    [Tooltip("可以站的地板，是否要撞到才能站")]
    public bool needHit;
    [Tooltip("會反彈的地板，設定反彈方向。設定Stop視為預設，彈回格子上。")]
    public Direction reboundDirection;
}

public class Ground : MonoBehaviour
{
    [Header("設定玩家從這個方向來時，玩家會有什麼行為。重複的方向後面的會無效。")]
    public GroundEvent[] groundEvents;

    //private Dictionary<Direction, GroundEvent> eventDictionary;

    void Awake()
    {
        //eventDictionary = new Dictionary<Vector2Int, GroundEvent>();
        //foreach (GroundEvent ge in groundEvents)
        //{
        //    switch (ge.inDirection)
        //    {
        //        case Direction.UP:
        //            if (!eventDictionary.ContainsKey(Vector2Int.up))
        //                eventDictionary[Vector2Int.up] = ge;
        //            break;
        //        case Direction.DOWN:
        //            if (!eventDictionary.ContainsKey(Vector2Int.down))
        //                eventDictionary[Vector2Int.down] = ge;
        //            break;
        //        case Direction.LEFT:
        //            if (!eventDictionary.ContainsKey(Vector2Int.left))
        //                eventDictionary[Vector2Int.left] = ge;
        //            break;
        //        case Direction.RIGHT:
        //            if (!eventDictionary.ContainsKey(Vector2Int.right))
        //                eventDictionary[Vector2Int.right] = ge;
        //            break;
        //    }
        //}
    }

    public GroundEvent GetGroundEvent(Vector2 direction)
    {
        direction.Normalize();
        Vector2Int dir = new Vector2Int((int)direction.x, (int)direction.y);
        Direction d;
        if (dir == Vector2Int.up)
            d = Direction.UP;
        else if (dir == Vector2Int.down)
            d = Direction.DOWN;
        else if (dir == Vector2Int.left)
            d = Direction.LEFT;
        else if (dir == Vector2Int.right)
            d = Direction.RIGHT;
        else
            d = Direction.Stop;
        foreach (GroundEvent ge in groundEvents)
        {
            if (ge.inDirection == d)
                return ge;
        }
        return null;
        //direction.Normalize();
        //Debug.Log("Get Ground Event d: " + direction);
        //Debug.Log("Dictionary = " + string.Join(", ", eventDictionary));
        //if (eventDictionary.ContainsKey(direction))
        //    return eventDictionary[direction];
        //else
        //    return null;
    }

    public Vector2 GetBoundPoint(Vector2 direction)
    {
        direction.Normalize();
        return (Vector2)transform.position + direction / 2.0f;
    }
}
