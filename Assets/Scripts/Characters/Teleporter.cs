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

    private AudioSource audioSource;

    private bool isActive;
    private bool hasTarget;

    private void Awake()
    {
        isActive = false;
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        ObjectTempoControl.Singleton.AddToBeatAction(Activate, activeTempo);
        ObjectTempoControl.Singleton.AddToBeatAction(Teleport, sendTempo);
    }

    private void Update()
    {
        if (isActive && isEntrance)
        {
            if (Physics2D.OverlapBox(transform.position, Vector2.one, 0, LayerMask.GetMask("Player")))
            {
                if (!hasTarget)
                {
                    Player.Singleton.movement.TeleportIn(this);
                    hasTarget = true;
                    audioSource.Play();
                }
            }
        }
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
        if (hasTarget)
        {
            Player.Singleton.movement.TeleportOut(exit);
            hasTarget = false;
            audioSource.Play();
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
