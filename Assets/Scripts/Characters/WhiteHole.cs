using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Characters;

public class WhiteHole : MonoBehaviour
{
    [Tooltip("For debug.")]
    public bool drawArea;
    // 範圍半徑
    public float radius;
    public TempoActionType actionType;
    // 範圍
    //public float radius;
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
                //Debug.Log("White Hole push player.");
                Vector2 position = Player.Singleton.transform.position;
                // 從左邊撞
                if (position.x >= transform.position.x + Constants.moveUnit / 2.0f)
                    Player.Singleton.movement.Knock(Vector2.right, pushUnit, pushSpeed);
                else if (position.x <= transform.position.x - Constants.moveUnit / 2.0f)
                    Player.Singleton.movement.Knock(Vector2.left, pushUnit, pushSpeed);
                else if (position.y >= transform.position.y + Constants.moveUnit / 2.0f)
                    Player.Singleton.movement.Knock(Vector2.up, pushUnit, pushSpeed);
                else if (position.y <= transform.position.y - Constants.moveUnit / 2.0f)
                    Player.Singleton.movement.Knock(Vector2.down, pushUnit, pushSpeed);
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

    private void OnDrawGizmos()
    {
        if (drawArea)
            Gizmos.DrawWireSphere(transform.position, radius);
    }
}
