using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public enum GroundBehavior
{
    [Tooltip("停下")]
    Stop,
    [Tooltip("可以站的")]
    Standable,
    [Tooltip("可以彈的")]
    Rebounce
}

[System.Serializable]
public class GroundEvent
{
    [Header("玩家撞到牆壁介於inDirectionLeft與inDirectionRight時要有的行為。")]
    [Tooltip("從哪個方向來。")]
    public Vector2 inDirectionLeft;
    [Tooltip("從哪個方向來。")]
    public Vector2 inDirectionRight;
    [Tooltip("從inDirection來時要有的行為。")]
    public GroundBehavior behavior;
    [Tooltip("可以站的地板，設定站的方向。")]
    public Vector2 standDirection;
    [Tooltip("會反彈的地板，設定反彈方向。")]
    public Vector2 reboundDirection;
}

public class Ground : MonoBehaviour
{
    [Header("設定玩家從這個方向來時，玩家會有什麼行為。")]
    public GroundEvent[] groundEvents;

    //private Dictionary<Direction, GroundEvent> eventDictionary;

    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
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

    private void ProcessEvent(Vector2 point)
    {
        Vector2 d = point - (Vector2)transform.position;
        int index = GetEventIndex(d);
        if (index != -1)
        {
            switch (groundEvents[index].behavior)
            {
                case GroundBehavior.Stop:
                    Debug.Log("Stop");
                    Player.Singleton.movement.StopMove();
                    break;
                case GroundBehavior.Standable:
                    //Debug.Log("Stand");
                    Player.Singleton.movement.StandOnGround(groundEvents[index].standDirection);
                    break;
                case GroundBehavior.Rebounce:
                    //Debug.Log("Rebounce");
                    Player.Singleton.movement.Knock(groundEvents[index].reboundDirection);
                    break;
                default:
                    Player.Singleton.movement.StopMove();
                    break;
            }
        }
    }

    private int GetEventIndex(Vector2 target)
    {
        int index = 0;
        foreach (GroundEvent ge in groundEvents)
        {
            //Debug.DrawRay(transform.position, ge.inDirectionLeft, Color.yellow, 5);
            //Debug.DrawRay(transform.position, ge.inDirectionRight, new Color(0.2f, 0.5f, 0.7f), 5);
            //Debug.DrawRay(transform.position, Vector3.Cross(ge.inDirectionLeft, target), Color.blue, 5);
            //Debug.Log(Vector3.Cross(ge.inDirectionLeft, target));
            //Debug.DrawRay(transform.position, Vector3.Cross(ge.inDirectionRight, target), Color.green, 5);
            //Debug.Log(Vector3.Cross(ge.inDirectionRight, target));
            if (Vector3.Cross(ge.inDirectionLeft, target).z < 0 && Vector3.Cross(ge.inDirectionRight, target).z > 0)
                return index;
            index++;
            //Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, 45) * ge.inDirectionRight, Color.green);
            index++;
        }
        return -1;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            ProcessEvent(collision.GetContact(0).point);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Color color = new Color(0, 1, 0);
        if (groundEvents == null)
            return;
        int index = 1;
        foreach (GroundEvent ge in groundEvents)
        {
            color += new Color(1.0f / groundEvents.Length, 0, 1.0f / groundEvents.Length);
            Gizmos.color = color;
            Vector2 left = ge.inDirectionLeft;
            Vector2 right = ge.inDirectionRight;
            Vector2 direction = (left + right).normalized;
            float angle = Mathf.Rad2Deg * Mathf.Acos(Vector2.Dot(left.normalized, right.normalized));
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + left);
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + right);
            Vector3 currentP = transform.position + (Vector3)right;
            Vector3 oldP;
            if (Vector3.Cross(left.normalized, right.normalized).z > 0)
                angle += 180;
            for (int i = 0; i <= angle; i++)
            {
                Vector3 d = Quaternion.AngleAxis(i, Vector3.forward) * right;
                oldP = currentP;
                currentP = transform.position + d.normalized * index * 0.25f;
                Gizmos.DrawLine(oldP, currentP);
                Gizmos.DrawLine(transform.position, currentP);
            }
            index++;
        }
    }
}
