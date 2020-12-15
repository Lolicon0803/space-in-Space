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
    [Tooltip("玩家會不會黏住，可以站的地板才有用")]
    public bool hasGravity;
    [Tooltip("會反彈的地板，設定反彈方向。")]
    public Vector2 reboundDirection;
}

public class Ground : MonoBehaviour
{
    [Header("設定玩家從這個方向來時，玩家會有什麼行為。")]
    public GroundEvent[] groundEvents;
    
    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("Ground");
    }

    /// <summary>
    /// 根據玩家撞到的點來處理事件
    /// </summary>
    /// <param name="point">撞到的點</param>
    private void ProcessEvent(Vector2 point)
    {
        Vector2 d = point - (Vector2)transform.position;
        int index = GetEventIndex(d);
        if (index != -1)
        {
            Debug.Log(System.Enum.GetName(typeof(GroundBehavior), groundEvents[index].behavior));
            switch (groundEvents[index].behavior)
            {
                case GroundBehavior.Stop:
                    Player.Singleton.movement.Knock(Vector2.zero);
                    Player.Singleton.movement.ZeroMoveDirection();
                    break;
                case GroundBehavior.Standable:
                    if (groundEvents[index].hasGravity && ! Player.Singleton.lifeSystem.IsDie)
                        Player.Singleton.transform.parent = transform;
                    Player.Singleton.movement.StandOnGround(groundEvents[index].standDirection);
                    break;
                case GroundBehavior.Rebounce:
                    Player.Singleton.movement.Knock(groundEvents[index].reboundDirection);
                    break;
                default:
                    Player.Singleton.movement.StopMove();
                    break;
            }
        }
        else
        {
            Player.Singleton.movement.Knock(Vector2.zero);
        }
    }

    /// <summary>
    /// 獲取正確的事件
    /// </summary>
    /// <param name="target">撞到的點</param>
    /// <returns></returns>
    private int GetEventIndex(Vector2 target)
    {
        int index = 0;
        // 一個一個找
        foreach (GroundEvent ge in groundEvents)
        {
            // 扇形範圍角度
            float angle = Vector2.SignedAngle(ge.inDirectionRight, ge.inDirectionLeft);
            if (angle < 0)
                angle += 360;
            // 玩家角度
            float targetAngle = Vector2.SignedAngle(ge.inDirectionRight, target);
            if (targetAngle < 0)
                targetAngle += 360;
            Debug.Log(targetAngle);
            // 小於表示在中間
            if (targetAngle < angle)
                return index;
            index++;
        }
        return -1;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player"))
        {
            Debug.DrawRay(transform.position, -Player.Singleton.movement.MoveDirection, Color.red, 10);
            ProcessEvent(Player.Singleton.transform.position);
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
            float angle = Vector2.SignedAngle(right, left); // Mathf.Rad2Deg * Mathf.Acos(Vector2.Dot(left.normalized, right.normalized));
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + left);
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + right);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.Cross(left, right));
            Vector3 currentP = transform.position + (Vector3)right;
            Vector3 oldP;
            if (angle < 0)
                angle += 360;
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
