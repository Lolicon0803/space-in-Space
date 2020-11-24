using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

static class Constants
{
    //世界座標移動單位
    public const float moveUnit = 1.0f;
}

public class PlayerMovement : MonoBehaviour
{
    // GameData Todo:之後要移動
    // 移動距離係數
    private readonly Dictionary<string, float> distanceDictionary = new Dictionary<string, float> { { "rocket", 2f }, { "move", 1f } };

    // 移動速度係數
    public float moveSpeed = 5f;

    // 滑行速度系數
    public float slideSpeed = 1f;

    // 噴射轉滑行的下降速度
    public float slowDownSpeed = 10.0f;

    // 速度改變係數
    private float speedChangeCoefficient = 1.0f;
    private float accumulatedCoefficient = 0.0f;

    private float nowSpeed;
    public float NowSpeed
    {
        get { return nowSpeed * speedChangeCoefficient; }
        private set { nowSpeed = value; }
    }

    // 下一個移動點
    public Vector2 movePoint;

    // 是否可以操作
    public bool canInput;

    // 是否第一次空拍
    //public bool firstTimeMiss;

    // 黑洞中，優先度最高
    public bool isBlackHole = false;

    private bool isStanding;

    private IEnumerator coroutineShoot;
    private IEnumerator coroutineSlide;
    private IEnumerator coroutineStandOnGround;

    // All player behavier.
    public delegate void PlayerBehavierDelegate(Vector2 direction);
    public event PlayerBehavierDelegate OnFireBag;
    public delegate void PlayerDamagedDelegate();
    public event PlayerDamagedDelegate OnStop;
    public event PlayerDamagedDelegate OnFallIntoBlackHole;
    public event PlayerDamagedDelegate OnError;

    public Vector2 MoveDirection { get; private set; }

    private new Rigidbody2D rigidbody;

    private LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        isStanding = false;
        rigidbody = GetComponent<Rigidbody2D>();
        groundLayer = LayerMask.GetMask("Ground");
        coroutineShoot = Shoot();
        coroutineSlide = Slide();
        coroutineStandOnGround = ShowStandOnGround(Vector2.zero);
        ResetStatus();
        StartCoroutine(ProcessOperation());
    }

    public void ResetStatus()
    {
        StopMove();
        canInput = true;
        isBlackHole = false;
        nowSpeed = 0;
        MoveDirection = Vector2.zero;
        movePoint = transform.position;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    private IEnumerator ProcessOperation()
    {
        while (true)
        {
            if (canInput)
            {
                // 滑鼠左鍵
                if (Input.GetMouseButtonDown(0))
                {
                    // 打在節拍上
                    if (TempoManager.Singleton.KeyDown())
                        HandleInput();
                    // 沒有打在節拍上且不在地上
                    else if (!isStanding)
                        OnError?.Invoke();
                }
            }
            yield return null;
        }
    }

    /// <summary>
    /// According to player's input to determine how will player move.
    /// </summary>
    private void HandleInput()
    {
        // 先終止移動，避免跑兩個IEnumerator
        StopMove();
        // 抓滑鼠位置算方向
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MoveDirection = mouse - (Vector2)transform.position;
        MoveDirection = MoveDirection.normalized;
        transform.parent = null;
        // 算移動角度轉角色
        float angle = Vector2.SignedAngle(Vector2.right, MoveDirection);
        transform.rotation = Quaternion.Euler(0, 0, angle);
        coroutineShoot = Shoot();
        OnFireBag?.Invoke(MoveDirection);
        isStanding = false;
        StartCoroutine(coroutineShoot);
    }

    private bool IsOnGround()
    {
        // 判斷自己下方是否有地板
        Collider2D c = Physics2D.Raycast(movePoint, -transform.up, Constants.moveUnit, groundLayer).collider;
        return c != null;
    }

    //private bool IsFrontHasGround()
    //{
    //    // 判斷前方一格下方是否有地板
    //    Collider2D c = Physics2D.Raycast(movePoint + new Vector2(Constants.moveUnit * MoveDirection.x, 0), -transform.up, Constants.moveUnit, obstacleLayer).collider;
    //    return c != null;
    //}

    /// <summary>
    /// Let player move to move point.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Shoot(float speed = -1, float distance = -1)
    {
        // 移動中，不可操作
        canInput = false;
        // 取得移動距離
        float totalDistance = (distance == -1) ? distanceDictionary["rocket"] : distance;
        if (speed == -1)
            nowSpeed = moveSpeed;
        else
            nowSpeed = speed;
        Debug.DrawLine(transform.position, (Vector2)transform.position + MoveDirection * totalDistance, Color.red, 3);
        rigidbody.velocity = MoveDirection * nowSpeed * speedChangeCoefficient;
        // 如果還沒動足夠距離，或是還有在動
        while (Vector2.Distance(transform.position, movePoint) < totalDistance && rigidbody.velocity.magnitude > Time.deltaTime)
        {
            // 給速度移動
            rigidbody.velocity = MoveDirection * nowSpeed * speedChangeCoefficient;
            yield return null;
        }
        movePoint = transform.position;
        canInput = true;
        StopCoroutine(coroutineSlide);
        StartCoroutine(coroutineSlide);
    }

    private IEnumerator Slide()
    {
        while (true)
        {
            // 慢慢變慢
            if (nowSpeed > slideSpeed)
                nowSpeed = Mathf.Lerp(nowSpeed, slideSpeed, slowDownSpeed * Time.deltaTime);
            rigidbody.velocity = MoveDirection * nowSpeed * speedChangeCoefficient;
            if (rigidbody.velocity.magnitude > Time.deltaTime)
                rigidbody.velocity = MoveDirection * nowSpeed * speedChangeCoefficient;
            else
                rigidbody.velocity = Vector2.zero;
            yield return null;
        }
    }

    /// <summary>
    /// 玩家往指定方向動，速度剩滑行速度。
    /// </summary>
    /// <param name="direction">方向，給0表示往玩家的反方向</param>
    public void Knock(Vector2 direction)
    {
        StopMove();
        isStanding = false;
        transform.parent = null;
        // 預設為玩家的反方向
        if (direction == Vector2.zero)
            MoveDirection = -MoveDirection;
        else
            MoveDirection = direction;
        canInput = true;
        nowSpeed = slideSpeed;
        StartCoroutine(coroutineSlide);
    }

    /// <summary>
    /// Force knock player.
    /// </summary>
    /// <param name="direction">推或吸的方向.零向量表示玩家反方向</param>
    /// <param name="impactFactor">推動或吸動幾個單位.</param>
    /// <param name="impactSpeed">推動或吸動速度.</param>
    public void Knock(Vector2 direction, float impactFactor, float impactSpeed)
    {
        StopMove();
        isStanding = false;
        // 預設為玩家的反方向
        if (direction == Vector2.zero)
            MoveDirection = -MoveDirection;
        else
            MoveDirection = direction;
        coroutineShoot = Shoot(impactSpeed, impactFactor);
        StartCoroutine(coroutineShoot);
    }

    /// <summary>
    /// For black hole. Perform fall into black hold effect and
    /// </summary>
    /// <param name="entrance"></param>
    public void FallIntoBlackHole(BlackHole entrance)
    {
        if (isBlackHole)
            return;
        isStanding = false;
        // Stop all movement.
        StopMove();
        isBlackHole = true;
        canInput = false;
        StartCoroutine(DisplayFallIntoBlackHole(entrance));
    }

    private IEnumerator DisplayFallIntoBlackHole(BlackHole entrance)
    {
        OnFallIntoBlackHole?.Invoke();

        movePoint = entrance.transform.position;
        // Rotate and move.
        while (Vector2.Distance(transform.position, movePoint) > entrance.impactSpeed * Time.deltaTime)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint, entrance.impactSpeed * Time.deltaTime);
            transform.Rotate(Vector3.forward, entrance.impactRotationSpeed * Time.deltaTime);
            yield return null;
        }
        // Rotate and become smaller then disappear.
        while (transform.localScale.magnitude > 0.1f * Time.deltaTime)
        {
            transform.Rotate(Vector3.forward, entrance.impactRotationSpeed * Time.deltaTime);
            transform.localScale = Vector2.MoveTowards(transform.localScale, Vector2.zero, 1 * Time.deltaTime);
            yield return null;
        }
        transform.localScale = Vector2.zero;
        canInput = false;
        //firstTimeMiss = true;
        GetComponent<PlayerLifeSystem>().GameOver();
    }

    /// <summary>
    /// Telepot player into entrance.
    /// </summary>
    /// <param name="entrance">Entrance.</param>
    public void TeleportIn(Teleporter entrance)
    {
        StopMove();
        isStanding = false;
        canInput = false;
        MoveDirection = Vector2.zero;
        StartCoroutine("DisplayTeleportIn", entrance);
    }

    /// <summary>
    /// Telepot player out exit.
    /// </summary>
    /// <param name="exit">Exit.</param>
    public void TeleportOut(Teleporter exit)
    {
        StopMove();
        isStanding = false;
        StopCoroutine("DisplayTeleportIn");
        transform.localScale = Vector2.zero;
        canInput = false;
        MoveDirection = Vector2.zero;
        StartCoroutine(DisplayTeleportOut(exit));
    }

    private IEnumerator DisplayTeleportIn(Teleporter entrance)
    {
        // Rotate and become smaller then disappear.
        while (transform.localScale.magnitude > 10.0f * Time.deltaTime)
        {
            transform.Rotate(Vector3.forward, entrance.impactRotationSpeed * Time.deltaTime);
            transform.localScale = Vector2.MoveTowards(transform.localScale, Vector2.zero, 10.0f * Time.deltaTime);
            yield return null;
        }
        transform.localScale = Vector2.zero;
    }

    private IEnumerator DisplayTeleportOut(Teleporter exit)
    {
        movePoint = exit.transform.position;
        transform.position = exit.transform.position;
        // Rotate, appear and become bigger.
        while (transform.localScale.magnitude < Vector2.one.magnitude)
        {
            transform.Rotate(Vector3.forward, exit.pushRotationSpeed * Time.deltaTime);
            transform.localScale = Vector2.MoveTowards(transform.localScale, Vector2.one, 10 * Time.deltaTime);
            yield return null;
        }
        transform.localScale = Vector2.one;
        transform.rotation = Quaternion.identity;
        Knock(exit.pushDirection, exit.pushUnit, exit.pushSpeed);
    }

    public void StandOnGround(Vector2 direction)
    {
        StopMove();
        isStanding = true;
        StopCoroutine(coroutineStandOnGround);
        coroutineStandOnGround = ShowStandOnGround(direction);
        StartCoroutine(coroutineStandOnGround);
    }

    public IEnumerator ShowStandOnGround(Vector2 direction)
    {
        Quaternion q = Quaternion.FromToRotation(-transform.up, direction);
        float angle = Vector2.SignedAngle(-transform.up, direction);
        if (angle != 0)
        {
            q.eulerAngles = new Vector3(0, 0, (transform.rotation.eulerAngles.z + q.eulerAngles.z) % 360);
            bool ok1 = true;
            bool ok2 = false;

            while (!ok1 || !ok2)
            {
                //if (collider2d.IsTouchingLayers(obstacleLayer))
                //{
                //    transform.position += -(Vector3)direction * Time.deltaTime;
                //    movePoint = transform.position;
                //}
                //else
                //    ok1 = true;
                float rotateSpeed = 1080f;
                if (Vector3.Distance(transform.rotation.eulerAngles, q.eulerAngles) > rotateSpeed * Time.deltaTime)
                {
                    if (q.eulerAngles.z > 0)
                        transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
                    else
                        transform.Rotate(-Vector3.forward, rotateSpeed * Time.deltaTime);
                }
                else
                    ok2 = true;
                yield return null;
            }
            transform.rotation = q;
        }
    }

    public void StopMove(bool enableInput = true)
    {
        StopCoroutine(coroutineShoot);
        StopCoroutine(coroutineSlide);
        OnStop?.Invoke();
        //MoveDirection = Vector2.zero;
        canInput = enableInput;
        //nowSpeed = 0;
        movePoint = transform.position;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero; 
    }

    public void Die()
    {
        StopMove(false);
        isBlackHole = false;
    }

    public void SpeedUp(float coefficient)
    {
        accumulatedCoefficient += coefficient;
        speedChangeCoefficient *= coefficient;
    }

    public void SpeedDown(float coefficient)
    {
        speedChangeCoefficient = 1;
        accumulatedCoefficient -= coefficient;
        speedChangeCoefficient *= (accumulatedCoefficient == 0) ? 1 : accumulatedCoefficient;
    }
}
