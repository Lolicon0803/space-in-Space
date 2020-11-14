using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Characters;

public class WhiteHole : MonoBehaviour
{
    // 範圍半徑
    public float radius;
    public TempoActionType actionType;
    // 推人推多遠
    public int pushUnit;
    // 推人時的速度
    public float pushSpeed;

    private Animator animator;
    private bool isActive;

    private LayerMask layerMask;

    private readonly int activateTrigger = Animator.StringToHash("Activate");

    private void Awake()
    {
        isActive = false;
        layerMask = LayerMask.GetMask("Player");
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        ObjectTempoControl.Singleton.AddToBeatAction(Activate, actionType);
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward, 90.0f * Time.deltaTime);
        if (isActive)
        {
            if (Physics2D.OverlapCircle(transform.position, radius, layerMask))
            {
                Vector2 direction = Player.Singleton.transform.position - transform.position;
                Player.Singleton.movement.Knock(direction, pushUnit, pushSpeed);
                isActive = false;
            }
        }
    }

    private void Activate()
    {
        //Debug.Log("White Hole activate");
        animator.SetTrigger(activateTrigger);
        isActive = true;
    }

    private void Deactivate()
    {
        //Debug.Log("White Hole dectivate");
        isActive = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
