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

    // 下一個移動點
    public Vector2 movePoint;

    // 四個碰撞點
    public Transform[] hitJudgmentPoints;

    // 是否可以操作
    public bool canInput;

    // 是否第一次空拍
    public bool firstTimeMiss;

    // 黑洞中，優先度最高
    public bool isBlackHole = false;

    private float distanceCoef = 0f;
    public float SpeedCoef { get; private set; }

    private Coroutine coroutineHitObstacle;
    private IEnumerator coroutineShoot;
    private IEnumerator coroutineSlide;

    private bool standing;
    private Vector2 standDirection;

    // 所有能阻擋玩家的層(玩家碰撞後回到格子中間)
    public LayerMask obstacleLayers;

    // All player behavier.
    public delegate void PlayerBehavierDelegate(Vector2 direction);
    public event PlayerBehavierDelegate OnWalk;
    public event PlayerBehavierDelegate OnFireBag;
    public delegate void PlayerDamagedDelegate();
    public event PlayerDamagedDelegate OnFallIntoBlackHole;
    public event PlayerDamagedDelegate OnMiss;
    public event PlayerDamagedDelegate OnError;

    private Vector2 moveDirection;
    private CapsuleCollider2D collider2d;

    // Start is called before the first frame update
    void Start()
    {
        collider2d = GetComponent<CapsuleCollider2D>();

        ObjectTempoControl.Singleton.AddToBeatAction(() =>
        {
            if (!IsOnGround() && canInput)
            {
                OnMiss?.Invoke();
                //Slide();
            }
        }, TempoActionType.TimeOut);

        SpeedCoef = 1.0f;
        moveDirection = Vector2.zero;
        firstTimeMiss = true;
        standing = false;
        standDirection = Vector2.down;
        movePoint = transform.position;
        canInput = true;
        StartCoroutine(ProcessOperation());
        coroutineShoot = Shoot();
        coroutineSlide = Slide();
    }

    private void Update()
    {
        //Debug.Log(SpeedCoef);
    }

    private IEnumerator ProcessOperation()
    {
        while (true)
        {
            if (canInput)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    // 打在節拍上
                    if (TempoManager.Singleton.KeyDown())
                        HandleInput();
                    // 沒有打在節拍上且不在地上
                    else if (!IsOnGround())
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
        // 抓滑鼠位置算方向
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 d = (mouse - (Vector2)transform.position).normalized;
        if (!standing || (standing && Mathf.Acos(Vector2.Dot(standDirection, d)) >= Mathf.PI / 2.0f))
        {
            standing = false;
            moveDirection = mouse - (Vector2)transform.position;
            moveDirection.Normalize();
            StopMove();
            coroutineShoot = Shoot();
            StartCoroutine(coroutineShoot);
        }
    }

    private bool IsOnGround()
    {
        // 判斷自己下方是否有地板
        Collider2D c = Physics2D.Raycast(movePoint, -transform.up, Constants.moveUnit, obstacleLayers).collider;
        return c != null;
    }

    private bool IsFrontHasGround()
    {
        // 判斷前方一格下方是否有地板
        Collider2D c = Physics2D.Raycast(movePoint + new Vector2(Constants.moveUnit * moveDirection.x, 0), -transform.up, Constants.moveUnit, obstacleLayers).collider;
        return c != null;
    }

    /// <summary>
    /// Let player move to move point.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Shoot(float speed = -1, float distance = -1)
    {
        // 移動中，不可操作
        canInput = false;
        // 預先設置目的地
        if (distance == -1)
            movePoint = (Vector2)transform.position + moveDirection * distanceDictionary["rocket"];
        else
            movePoint = (Vector2)transform.position + moveDirection * distance;
        if (speed == -1)
            SpeedCoef = moveSpeed;
        else
            SpeedCoef = speed;
        Debug.DrawLine(transform.position, movePoint, Color.red, 3);
        // 直到到達目的地為止
        while (Vector2.Distance(transform.position, movePoint) > SpeedCoef * Time.deltaTime)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint, SpeedCoef * Time.deltaTime);
            yield return null;
        }
        //Debug.Log("Shoot Finish");
        transform.position = movePoint;
        canInput = true;
        StopCoroutine("Slide");
        StartCoroutine("Slide");
    }

    private IEnumerator Slide()
    {
        SpeedCoef = slideSpeed;
        while (true)
        {
            transform.position += (Vector3)moveDirection * SpeedCoef * Time.deltaTime;
            movePoint = transform.position;
            yield return null;
        }
    }

    /// <summary>
    /// 玩家往指定方向動，速度剩滑行速度。
    /// </summary>
    /// <param name="direction">方向，給0表示往玩家的反方向</param>
    public void Knock(Vector2 direction)
    {
        if (direction == Vector2.zero)
            moveDirection = -moveDirection;
        else
            moveDirection = direction;
        StopMove();
        canInput = true;
        StartCoroutine("Slide");
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
        if (direction == Vector2.zero)
            moveDirection = -moveDirection;
        else
            moveDirection = direction;
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
        // Stop all movement.
        StopMove();
        isBlackHole = true;
        canInput = false;
        moveDirection = Vector2.zero;
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
        firstTimeMiss = true;
        GetComponent<PlayerLifeSystem>().GameOver();
    }

    /// <summary>
    /// Telepot player into entrance.
    /// </summary>
    /// <param name="entrance">Entrance.</param>
    public void TeleportIn(Teleporter entrance)
    {
        StopMove();
        canInput = false;
        moveDirection = Vector2.zero;
        StartCoroutine("DisplayTeleportIn", entrance);
    }

    /// <summary>
    /// Telepot player out exit.
    /// </summary>
    /// <param name="exit">Exit.</param>
    public void TeleportOut(Teleporter exit)
    {
        StopMove();
        StopCoroutine("DisplayTeleportIn");
        transform.localScale = Vector2.zero;
        canInput = false;
        moveDirection = Vector2.zero;
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

    public void ResetStatus()
    {
        canInput = true;
        isBlackHole = false;
        firstTimeMiss = true;
        StopCoroutine(coroutineShoot);
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        movePoint = transform.position;
    }

    public void Die()
    {
        StopMove();
        canInput = false;
        isBlackHole = false;
        firstTimeMiss = true;
    }

    public void StandOnGround(Vector2 direction)
    {
        standing = true;
        standDirection = direction;
        StopMove();
        StartCoroutine("ShowStandOnGround", direction);
    }

    public IEnumerator ShowStandOnGround(Vector2 direction)
    {
        Quaternion q = Quaternion.FromToRotation(-transform.up, direction);
        if (q.eulerAngles.z != 0)
        {
            q.eulerAngles = new Vector3(0, 0, (transform.rotation.eulerAngles.z + q.eulerAngles.z) % 360);
            bool ok1 = false;
            bool ok2 = false;
            ContactFilter2D filter = new ContactFilter2D()
            {
                layerMask = LayerMask.GetMask("Ground")
            };
            Collider2D[] colliders = new Collider2D[1];
            while (!ok1 || !ok2)
            {
                if (collider2d.OverlapCollider(filter, colliders) > 0)
                {
                    transform.position += -(Vector3)direction * Time.deltaTime;
                    movePoint = transform.position;
                }
                else
                    ok1 = true;
                if (Vector3.Distance(transform.rotation.eulerAngles, q.eulerAngles) > 5.625f * Time.deltaTime)
                {
                    if (q.eulerAngles.z > 0)
                        transform.Rotate(Vector3.forward, 5.625f);
                    else
                        transform.Rotate(-Vector3.forward, 5.625f);
                }
                else
                    ok2 = true;
                yield return null;
            }
            //Debug.Log("Start4");
            transform.rotation = q;
        }
    }

    public void StopMove()
    {
        StopCoroutine(coroutineShoot);
        StopCoroutine("Slide");
        SpeedCoef = 0;
        movePoint = transform.position;
        canInput = true;
    }
}
