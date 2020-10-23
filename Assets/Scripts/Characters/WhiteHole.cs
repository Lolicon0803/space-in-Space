using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Characters;

public class WhiteHole : MonoBehaviour
{
    public TempoActionType actionType;
    // 範圍
    //public float radius;
    // 推人推多遠
    public int pushUnit;
    // 推人時的速度
    public float pushSpeed;

    private Animator animator;
    private bool isActive;

    private readonly int activateTrigger = Animator.StringToHash("Activate");

    private void Awake()
    {
        isActive = false;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        ObjectTempoControl.Singleton.AddToBeatAction(Activate, actionType);
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward, 90.0f * Time.deltaTime);
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
        if (!isActive)
            return;

        if (collision.CompareTag("Player"))
        {
            //Debug.Log("White Hole push player.");
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            Vector2 position = collision.transform.position;
            float radius = GetComponent<CircleCollider2D>().radius;
            // 從左邊撞
            if (position.x >= transform.position.x + radius / 2.0f * Constants.moveUnit)
                player.Knock(Vector2.right, pushUnit, pushSpeed);
            else if (position.x <= transform.position.x - radius / 2.0f * Constants.moveUnit)
                player.Knock(Vector2.left, pushUnit, pushSpeed);
            else if (position.y >= transform.position.y + radius / 2.0f * Constants.moveUnit)
                player.Knock(Vector2.up, pushUnit, pushSpeed);
            else if (position.y <= transform.position.y - radius / 2.0f * Constants.moveUnit)
                player.Knock(Vector2.down, pushUnit, pushSpeed);
        }
    }
}
