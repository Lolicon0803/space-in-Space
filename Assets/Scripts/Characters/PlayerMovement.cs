using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerDirection
{
    None = 0,
    Up = 0b_00001,
    Down = 0b_00010,
    Left = 0b_00100,
    Right = 0b_01000,
}

public class PlayerMovement : MonoBehaviour
{
    public GameObject groundDetecter;
    // When to start slow.
    public float waitForSlowDownTime;
    // Total move time.
    public float moveTime;
    // Push power, The bigger this value is, The more player's speed is fast.
    public float pushPower;

    [Tooltip("When press up/down arrow and push, the factor for push power to multiply.")]
    public Vector2 pushVerticalFactor;
    [Tooltip("When press left or right arrow and push, the factor for push power to multiply.")]
    public Vector2 pushHorizontalFactor;
    [Tooltip("When press up arrow with left or right arrow and push, the factor for push power to multiply.")]
    public Vector2 pushDiagonalFactor;

    private Rigidbody2D rigid;

    private PlayerDirection direction;
    private PlayerDirection preDirection;
    private bool movePressed;
    private bool jumpPressed;
    private bool isGround;

    private void Awake()
    {
        direction = PlayerDirection.None;
        preDirection = PlayerDirection.None;
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
            direction = PlayerDirection.Left;
            movePressed = true;
        }
        // Move right.
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            direction = PlayerDirection.Right;
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
            jumpPressed = true;
        }
    }

    private void FixedUpdate()
    {
        isGround = Physics2D.OverlapCircle(groundDetecter.transform.position, 0.1f, LayerMask.GetMask("Ground"));
        // Check tempo.
        // bool successful = Check tempo by player's input.
        Push(true);
    }

    private void Push(bool successful)
    {
        if (successful)
        {
            if (jumpPressed)
            {
                if (direction == PlayerDirection.Up)
                {
                    rigid.velocity = new Vector2(0, pushPower * pushVerticalFactor.x);
                    StopCoroutine("ControlPush");
                    StartCoroutine("ControlPush");
                }
                else if (direction == PlayerDirection.Left)
                {
                    rigid.velocity = new Vector2(-pushPower * pushHorizontalFactor.x, 0);
                    StopCoroutine("ControlPush");
                    StartCoroutine("ControlPush");
                }
                else if (direction == PlayerDirection.Right)
                {
                    rigid.velocity = new Vector2(pushPower * pushHorizontalFactor.x, 0);
                    StopCoroutine("ControlPush");
                    StartCoroutine("ControlPush");
                }
            }

            //switch (direction)
            //{
            //    case PlayerDirection.Up:
            //        Debug.Log("Move");
            //        rigid.velocity = new Vector2(0, pushPower * pushVerticalFactor);
            //        break;
            //    case PlayerDirection.Down:
            //        break;
            //    //case PlayerDirection.Left:
            //    //    rigid.velocity = new Vector2(-pushPower * pushHorizontalFactor.x / 2.0f, 0);
            //    //    break;
            //    //case PlayerDirection.Right:
            //    //    rigid.velocity = new Vector2(pushPower * pushHorizontalFactor.x / 2.0f, 0);
            //    //    break;
            //    case PlayerDirection.Left:
            //        Debug.Log("Move");
            //        rigid.velocity = new Vector2(-pushPower * pushHorizontalFactor.x, 0);
            //        break;
            //    case PlayerDirection.Right:
            //        Debug.Log("Move");
            //        rigid.velocity = new Vector2(pushPower * pushHorizontalFactor.x, 0);
            //        break;
            //    //case PlayerDirection.Up | PlayerDirection.Right:
            //    //    rigid.velocity = new Vector2(pushPower * pushDiagonalFactor.x, pushPower * pushDiagonalFactor.y);
            //    //    break;
            //    //case PlayerDirection.Up | PlayerDirection.Left:
            //    //    rigid.velocity = new Vector2(-pushPower * pushDiagonalFactor.x, pushPower * pushDiagonalFactor.y);
            //    //    break;
            //    default:
            //        break;
            //}
        }
        isGround = false;
        jumpPressed = false;
    }

    private IEnumerator ControlPush()
    {
        // Player is pushing.
        yield return new WaitForSeconds(waitForSlowDownTime);
        float t = 0;
        float leftTime = moveTime - waitForSlowDownTime;
        float fps = 1.0f / Time.deltaTime;
        float dx = rigid.velocity.x / (leftTime * fps);
        float dy = rigid.velocity.y / (leftTime * fps);
        while(t < leftTime)
        {
            Vector2 v = rigid.velocity;
            v.x -= v.x == 0 ? 0 : dx;
            v.y -= v.y == 0 ? 0 : dy;
            rigid.velocity = v;
            yield return null;
            t += Time.deltaTime;
        }
        rigid.velocity = Vector2.zero;
    }
}
