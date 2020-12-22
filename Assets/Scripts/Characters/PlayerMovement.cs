using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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

    private float nowSpeed;
    public float NowSpeed
    {
        get { return nowSpeed; }
        private set { nowSpeed = value; }
    }

    [Header("偵測地板的射線起點(星球用)")]
    public Transform groundCheck;
    public float distance = 0f;

    // 是否在有重力的星球上
    public bool isOnPlanet = false;

    // 現在位置
    public Vector2 movePoint;

    // 是否可以操作
    public bool canInput;

    // 是否第一次空拍
    public bool firstTimeMiss;
    // 是否移動過
    public bool notMoveYet;
    // 黑洞中，優先度最高
    public bool isBlackHole = false;
    // 是否正在移動(移動中無視懲罰)
    public bool isMoving;

    private bool isDie;

    private IEnumerator coroutineShoot;

    // All player behavier.
    public delegate void PlayerBehavierDelegate(Vector2 direction);
    public event PlayerBehavierDelegate OnFireBag;
    public delegate void PlayerDamagedDelegate();
    public event PlayerDamagedDelegate OnStop;
    public event PlayerDamagedDelegate OnFallIntoBlackHole;
    public event PlayerDamagedDelegate OnError;

    private PlayerAnimationManager animationManager;

    public Vector2 MoveDirection { get; private set; }

    private LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        animationManager = GetComponent<PlayerAnimationManager>();
        groundLayer = LayerMask.GetMask("Ground");
        coroutineShoot = Shoot();
        ResetStatus();
        if (ObjectTempoControl.Singleton != null)
        {
            ObjectTempoControl.Singleton.AddToBeatAction(() =>
            {
                if (canInput && !isDie && !notMoveYet)
                {
                    if (firstTimeMiss == false)
                        OnError?.Invoke();
                    firstTimeMiss = false;
                    Punish();
                }
            }, TempoActionType.TimeOut);
        }
        // 根據bpm改變移動速度
        if (TempoManager.Singleton != null)
        {
            if (TempoManager.Singleton.beatPerMinute > 60)
            {
                moveSpeed *= (float)TempoManager.Singleton.beatPerMinute / 60.0f;
                slideSpeed *= (float)TempoManager.Singleton.beatPerMinute / 60.0f;
            }
        }
        // 對話完後玩家可以輸入
        TextWriter tw = FindObjectOfType<TextWriter>();
        if (tw != null)
        {
            tw.onEndStory.AddListener(() =>
            {
                notMoveYet = true;
                canInput = true;
            });
        }
        // 切場景後，重新註冊節拍事件
        SceneManager.sceneLoaded += RegisterTempo;
        if (ObjectTempoControl.Singleton != null)
            StartCoroutine(ProcessOperation());
    }

    public bool IsGroundOnPlanet
    {
        get
        {
            Vector2 start = groundCheck.position;
            Vector2 end = new Vector2(start.x, start.y - distance);

            //Debug.DrawLine(start, end, Color.blue);
            bool grounded = Physics2D.Linecast(start, end, groundLayer);
            return grounded;
        }
    }

    public bool IsGroundOnPlanetForFalling
    {
        get
        {
            Vector2 start = groundCheck.position;
            Vector2 end = new Vector2(start.x, start.y - 0.01f);
            //Debug.DrawLine(start, end, Color.blue);
            bool grounded = Physics2D.Linecast(start, end, groundLayer);
            return grounded;
        }
    }

    private void RegisterTempo(Scene arg0, LoadSceneMode arg1)
    {
        if (ObjectTempoControl.Singleton != null)
        {
            ObjectTempoControl.Singleton.AddToBeatAction(() =>
            {
                if (canInput && !isDie && !notMoveYet)
                {
                    if (firstTimeMiss == false)
                        OnError?.Invoke();
                    firstTimeMiss = false;
                    Punish();
                }
            }, TempoActionType.TimeOut);
            // 根據bpm改變移動速度
            if (TempoManager.Singleton.beatPerMinute > 60)
            {
                moveSpeed *= (float)TempoManager.Singleton.beatPerMinute / 60.0f;
                slideSpeed *= (float)TempoManager.Singleton.beatPerMinute / 60.0f;
            }
        }
        // 對話完後玩家可以輸入
        TextWriter tw = FindObjectOfType<TextWriter>();
        if (tw != null)
            tw.onEndStory.AddListener(() => { canInput = true; });
    }

    public void ResetStatus()
    {
        StopMove(true);
        isBlackHole = false;
        firstTimeMiss = true;
        notMoveYet = true;
        isDie = false;
        isMoving = false;
        nowSpeed = 0;
        MoveDirection = Vector2.zero;
        movePoint = transform.position;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        GetComponent<Rigidbody2D>().simulated = true;
    }

    private IEnumerator ProcessOperation()
    {
        while (true)
        {
            if (!isOnPlanet && canInput && !isDie)
            {
                float x = 0;
                float y = 0;
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                    x = -1;
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                    x = 1;
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                    y = 1;
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                    y = -1;
                // 有移動
                if (x != 0 || y != 0)
                {
                    notMoveYet = false;
                    // 打在節拍上
                    if (TempoManager.Singleton.KeyDown())
                    {
                        firstTimeMiss = true;
                        MoveDirection = new Vector2(x, y);
                        HandleInput();
                    }
                    // 打錯拍點
                    else
                    {
                        if (firstTimeMiss == false)
                            OnError?.Invoke();
                        firstTimeMiss = false;
                        Punish();
                    }
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
        StopCoroutine(coroutineShoot);
        transform.parent = null;
        // 噴射事件
        OnFireBag?.Invoke(MoveDirection);
        if (MoveDirection.x == 1)
            transform.rotation = Quaternion.identity;
        else if (MoveDirection.x == -1)
            transform.rotation = Quaternion.Euler(180, 0, 180);
        else if (MoveDirection.y == 1)
            transform.rotation = Quaternion.Euler(0, 0, 90);
        else if (MoveDirection.y == -1)
            transform.rotation = Quaternion.Euler(0, 0, 270);
        coroutineShoot = Shoot();
        StartCoroutine(coroutineShoot);
    }

    private void Punish()
    {
        if (!isMoving)
        {
            Debug.Log("懲罰 + " + MoveDirection);
            Knock(MoveDirection, 1, slideSpeed);
        }
    }

    /// <summary>
    /// Let player move to movePoint.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Shoot(bool enableInput = false, float speed = -1, float distance = -1)
    {
        isMoving = true;
        yield return null;
        // 移動中，不可操作
        canInput = enableInput;
        // 取得移動距離
        float totalDistance = (distance == -1) ? distanceDictionary["rocket"] : distance;
        if (speed == -1)
            nowSpeed = moveSpeed;
        else
            nowSpeed = speed;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, MoveDirection, totalDistance, groundLayer);
        //撞牆情況
        if (hit.collider != null)
        {
            RaycastHit2D hit1 = Physics2D.Raycast(hit.point, -MoveDirection, 5, LayerMask.GetMask("Player"));
            float d = Vector2.Distance(hit.point, hit1.point);
            movePoint = (Vector2)transform.position + MoveDirection * (d + 0.05f);
        }
        // 一般情況
        else
        {
            movePoint = (Vector2)transform.position + MoveDirection * totalDistance;
            // 校正回格子上
            movePoint.x = Mathf.Floor(movePoint.x) + 0.5f;
            movePoint.y = Mathf.Floor(movePoint.y) + 0.5f;
        }
        // 直到抵達movePoint
        while (Vector2.Distance(transform.position, movePoint) > nowSpeed * Time.deltaTime)
        {
            // 移動
            transform.position = Vector2.MoveTowards(transform.position, movePoint, nowSpeed * Time.deltaTime);
            yield return null;
        }
        // 移動結束
        transform.position = movePoint;
        canInput = true;
        isMoving = false;
    }

    /// <summary>
    /// 玩家往指定方向動，速度剩滑行速度。
    /// </summary>
    /// <param name="direction">方向，給0表示往玩家的反方向</param>
    public void Knock(Vector2 direction, bool enableInput = false)
    {
        if (isDie || isBlackHole)
            return;
        StopCoroutine(coroutineShoot);
        //StopMove(enableInput);
        transform.parent = null;
        float distance = 0;
        // 預設為玩家的反方向
        if (direction == Vector2.zero)
            MoveDirection = -MoveDirection;
        else
        {
            MoveDirection = direction;
            distance = 1;
        }
        Debug.Log("推: " + MoveDirection);
        nowSpeed = slideSpeed;
        coroutineShoot = Shoot(enableInput, slideSpeed, distance);
        StartCoroutine(coroutineShoot);
    }

    /// <summary>
    /// Force knock player.
    /// </summary>
    /// <param name="direction">推或吸的方向.零向量表示玩家反方向</param>
    /// <param name="knockDistance">推動或吸動幾個單位.</param>
    /// <param name="knockSpeed">推動或吸動速度.</param>
    public void Knock(Vector2 direction, float knockDistance, float knockSpeed, bool enableInput = false)
    {
        if (isDie || isBlackHole)
            return;
        StopCoroutine(coroutineShoot);
        //StopMove(enableInput);
        // 預設為玩家的反方向
        if (direction == Vector2.zero)
            MoveDirection = -MoveDirection;
        else
            MoveDirection = direction;
        Debug.Log("推: " + MoveDirection);
        coroutineShoot = Shoot(enableInput, knockSpeed, knockDistance);
        StartCoroutine(coroutineShoot);
    }

    public void ZeroMoveDirection()
    {
        MoveDirection = Vector2.zero;
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
        StopMove(false);
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
        MoveDirection = Vector2.zero;
        Quaternion q = Quaternion.FromToRotation(-transform.up, direction);
        q.eulerAngles = new Vector3(0, 0, (transform.rotation.eulerAngles.z + q.eulerAngles.z) % 360);
        transform.rotation = q;
        animationManager.PlayLie();
    }

    public void BackToGrid()
    {
        Vector2 now = transform.position;
        now.x = Mathf.Floor(now.x) + 0.5f;
        now.y = Mathf.Floor(now.y) + 0.5f;
        transform.position = now;
    }

    public void StopMove(bool enableInput = true)
    {
        StopCoroutine(coroutineShoot);
        OnStop?.Invoke();
        MoveDirection = Vector2.zero;
        canInput = enableInput;
        nowSpeed = 0;
        movePoint = transform.position;
    }

    public void Die()
    {
        GetComponent<Rigidbody2D>().simulated = false;
        isDie = true;
        transform.parent = null;
        StopMove(false);
        StopCoroutine(coroutineShoot);
        isBlackHole = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(movePoint, 1);
    }
}
