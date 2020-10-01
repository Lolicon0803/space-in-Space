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

public class Player : Character
{
    public float moveSpeed;
    public float pushPower;

    private Rigidbody2D rigid;

    private bool isGotHit;
    private bool isPushing;
    private PlayerDirection direction;

    private Coroutine pushCoroutine;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        rigid = GetComponent<Rigidbody2D>();
        isGotHit = false;
        StartCoroutine(Move());
        direction = PlayerDirection.None;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator Move()
    {
        while (true)
        {
            //Debug.Log(rigid.velocity);
            if (!isGotHit)
            {
                // Move left.
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    if (!isPushing && rigid.velocity.y == 0)
                        rigid.velocity = new Vector2(-moveSpeed * Time.deltaTime, 0);
                    transform.localScale = new Vector3(-1, 1, 1);
                    direction = (direction & 0 << (int)PlayerDirection.Right) | PlayerDirection.Left;
                }
                // Move right.
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    if (!isPushing && rigid.velocity.y == 0)
                        rigid.velocity = new Vector2(moveSpeed * Time.deltaTime, 0);
                    transform.localScale = new Vector3(1, 1, 1);
                    direction = (direction & 0 << (int)PlayerDirection.Left) | PlayerDirection.Right;
                }
                //Stop.
                else
                {
                    direction = PlayerDirection.None;
                }

                // Up
                if (Input.GetKey(KeyCode.UpArrow))
                    direction = direction | PlayerDirection.Up;
                // Down.
                else if (Input.GetKey(KeyCode.DownArrow))
                    direction = direction | PlayerDirection.Down;
            }
            // Use rocket backpack.
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Check tempo.
                if (!isPushing)
                {
                    if (pushCoroutine != null)
                        StopCoroutine(pushCoroutine);
                    switch (direction)
                    {
                        case PlayerDirection.None:
                            pushCoroutine = StartCoroutine(Push(0, 1));
                            break;
                        case PlayerDirection.Up:
                            pushCoroutine = StartCoroutine(Push(0, 1.5f));
                            break;
                        case PlayerDirection.Down:
                            pushCoroutine = StartCoroutine(Push(0, -1));
                            break;
                        case PlayerDirection.Left:
                            pushCoroutine = StartCoroutine(Push(-0.5f, 0.5f));
                            break;
                        case PlayerDirection.Right:
                            pushCoroutine = StartCoroutine(Push(0.5f, 0.5f));
                            break;
                        case PlayerDirection.Up | PlayerDirection.Right:
                            pushCoroutine = StartCoroutine(Push(0.75f, 1.2f));
                            break;
                        case PlayerDirection.Up | PlayerDirection.Left:
                            pushCoroutine = StartCoroutine(Push(-0.75f, 1.2f));
                            break;
                        default:
                            break;
                    }
                }
            }
            yield return null;
        }
    }

    private IEnumerator Push(float x, float y)
    {
        rigid.velocity = Vector2.zero;
        isPushing = true;
        float power = pushPower * Time.deltaTime;
        rigid.AddForce(new Vector2(power * x, power * y));
        yield return null;
        isPushing = false;
    }

    //public void Damaged(Vector2 force)
    //{
    //    isGotHit = true;
    //    force *= Time.deltaTime;
    //    rigid.AddForce(new Vector2(-transform.localScale.x * force.x, force.y));
    //    StartCoroutine(RecoverHit());
    //}

    //private IEnumerator RecoverHit()
    //{
    //    yield return new WaitForSeconds(1.5f);
    //    isGotHit = false;
    //}
}
