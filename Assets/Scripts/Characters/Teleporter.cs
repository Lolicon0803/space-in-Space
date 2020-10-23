using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    // 作用節奏
    public TempoActionType activeTempo;
    // 是入口
    public bool isEntrance;
    // 出口
    public Teleporter exit;
    // 傳送節奏(幾拍後人出現)
    public TempoActionType sendTempo;
    // 是出口
    public bool isExit;
    // 推人推多遠
    public int pushUnit;
    // 推人時的速度
    public float pushSpeed;
    // 吸人時人的旋轉速度
    public float impactRotationSpeed;
    // 推人時人的旋轉速度
    public float pushRotationSpeed;
    // 推人方向
    public Vector2 pushDirection;

    private PlayerMovement target;

    private bool isActive;

    private void Awake()
    {
        isActive = false;
        target = null;
    }

    private void Start()
    {
        ObjectTempoControl.Singleton.AddToBeatAction(Activate, activeTempo);
        ObjectTempoControl.Singleton.AddToBeatAction(Teleport, sendTempo);
    }

    private void Activate()
    {
        isActive = true;
    }

    private void Deactivate()
    {
        isActive = false;
    }

    private void Teleport()
    {
        if (target != null)
        {
            target.TeleportOut(exit);
            target = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive)
            return;

        if (collision.CompareTag("Player"))
        {
            target = collision.GetComponent<PlayerMovement>();
            if (Vector2.Distance(transform.position, target.movePoint.position) <= Constants.moveUnit)
                target.TeleportIn(this);
        }
    }

    private void OnDrawGizmos()
    {
        if (isEntrance)
        {
            if (exit == null)
                Debug.LogError("Telepoter " + gameObject.name + " has no exit.");
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, exit.transform.position);
        }
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + pushUnit * (Vector3)pushDirection);
    }
}
