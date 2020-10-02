using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerDirection
{
    None = 0,
    Up = 1,
    Down = 2,
    Left = 4,
    Right = 8,
}

public class PlayerMovement : MonoBehaviour
{
    public GameObject groundDetecter;
    public float moveSpeed;
    public float pushPower;

    [Tooltip("When press up arrow and push, the factor for push power to multiply.")]
    public float pushUpFactor;
    [Tooltip("When press left or right arrow and push, the factor for push power to multiply.")]
    public Vector2 pushHorizontalFactor;
    [Tooltip("When press up arrow with left or right arrow and push, the factor for push power to multiply.")]
    public Vector2 pushDiagonalFactor;

    private Rigidbody2D rigid;

    private PlayerDirection direction;
    private bool movePressed;
    private bool jumpPressed;
    private bool isGround;

    private void Awake()
    {
        direction = PlayerDirection.None;
        movePressed = false;
        jumpPressed = false;
        isGround = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move left.
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            direction = (direction & 0 << (int)PlayerDirection.Right) | PlayerDirection.Left;
            movePressed = true;
        }
        // Move right.
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            direction = (direction & 0 << (int)PlayerDirection.Left) | PlayerDirection.Right;
            movePressed = true;
        }
        //Stop.
        else
        {
            direction = PlayerDirection.None;
            movePressed = false;
        }

        // Up
        if (Input.GetKey(KeyCode.UpArrow))
            direction = direction | PlayerDirection.Up;
        // Down.
        else if (Input.GetKey(KeyCode.DownArrow))
            direction = direction | PlayerDirection.Down;

        // Use rocket backpack.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Check tempo.
            if (true)
            {
                jumpPressed = true;
            }
        }
    }

    private void FixedUpdate()
    {
        isGround = Physics2D.OverlapCircle(groundDetecter.transform.position, 0.1f, LayerMask.GetMask("Ground"));
        if (isGround)
            rigid.velocity = Vector2.zero;
        Move();
        Push();
    }

    private void Move()
    {
        if (movePressed && isGround)
        {
            if ((direction & PlayerDirection.Right) != 0)
                rigid.velocity = new Vector2(moveSpeed, rigid.velocity.y);
            else if ((direction & PlayerDirection.Left) != 0)
                rigid.velocity = new Vector2(-moveSpeed, rigid.velocity.y);
        }
    }

    private void Push()
    {
        if (jumpPressed)
        {
            switch (direction)
            {
                case PlayerDirection.None:
                    rigid.velocity = new Vector2(0, pushPower);
                    break;
                case PlayerDirection.Up:
                    rigid.velocity = new Vector2(0, pushPower * pushUpFactor);
                    break;
                case PlayerDirection.Down:
                    break;
                case PlayerDirection.Left:
                    //rigid.velocity = new Vector2(-pushPower * pushHorizontalFactor.x, rigid.velocity.y + pushPower * pushHorizontalFactor.y);
                    rigid.velocity = new Vector2(-pushPower * pushHorizontalFactor.x, pushPower * pushHorizontalFactor.y);
                    break;
                case PlayerDirection.Right:
                    //rigid.velocity = new Vector2(pushPower * pushHorizontalFactor.x, rigid.velocity.y + pushPower * pushHorizontalFactor.y);
                    rigid.velocity = new Vector2(pushPower * pushHorizontalFactor.x, pushPower * pushHorizontalFactor.y);
                    break;
                case PlayerDirection.Up | PlayerDirection.Right:
                    rigid.velocity = new Vector2(pushPower * pushDiagonalFactor.x, pushPower * pushDiagonalFactor.y);
                    break;
                case PlayerDirection.Up | PlayerDirection.Left:
                    rigid.velocity = new Vector2(-pushPower * pushDiagonalFactor.x, pushPower * pushDiagonalFactor.y);
                    break;
                default:
                    break;
            }
            jumpPressed = false;
        }
    }

    private IEnumerator ControlJump()
    {
        yield return null;
    }
}
