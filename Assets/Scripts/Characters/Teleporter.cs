using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    // 作用節奏
    public TempoActionType activeTempo;
    [Tooltip("是入口")]
    public bool isEntrance;
    [Tooltip("出口")]
    public Teleporter exit;
    [Tooltip("傳送節奏(幾拍後人出現)")]
    public TempoActionType sendTempo;
    [Tooltip("是出口")]
    public bool isExit;
    [Tooltip("出口推人推多遠")]
    public int pushUnit;
    [Tooltip("出口推人時的速度")]
    public float pushSpeed;
    [Tooltip("吸人時人的旋轉速度")]
    public float impactRotationSpeed;
    [Tooltip("推人時人的旋轉速度")]
    public float pushRotationSpeed;
    [Tooltip(" 推人方向")]
    public Vector2 pushDirection;

    private AudioSource audioSource;

    private bool isActive;
    private bool hasTarget;

    private void Awake()
    {
        isActive = true;
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        //ObjectTempoControl.Singleton.AddToBeatAction(Activate, activeTempo);
        ObjectTempoControl.Singleton.AddToBeatAction(Teleport, sendTempo);
    }

    private void Update()
    {
        if (isActive && isEntrance)
        {
            if (Physics2D.OverlapBox(transform.position, Vector2.one, 0, LayerMask.GetMask("Player")))
            {
                PlayerMovement player = Player.Singleton.movement;
                if (Vector2.Distance(player.transform.position, transform.position) < player.moveSpeed * Time.deltaTime)
                {
                    if (!hasTarget && !player.isTeleporting)
                    {
                        
                        Player.Singleton.movement.TeleportIn(this);
                        hasTarget = true;
                        audioSource.Play();
                    }
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
