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

    private Vector2 oldMoveVector;
    public Vector2 OldMoveVector
    {
        get { return oldMoveVector; }
        private set { oldMoveVector = value; }
    }
    private float distanceCoef = 0f;
    public float SpeedCoef { get; private set; }

    private Coroutine coroutineHitObstacle;
    private Coroutine coroutineShoot;
    private Coroutine coroutineSlide;

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
    private CapsuleCollider2D collider;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<CapsuleCollider2D>();

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
        movePoint = transform.position;
        canInput = true;
        StartCoroutine(ProcessOperation());
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
                //CheckInput();
                //if (inputDirection != Vector2.zero)
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
    /// Get player's keyboard input.
    /// </summary>
    /// <returns></returns>
    //private void CheckInput()
    //{
    //    if (Input.GetKeyDown(KeyCode.LeftArrow))
    //        inputDirection = -transform.right;
    //    else if (Input.GetKeyDown(KeyCode.RightArrow))
    //        inputDirection = transform.right;
    //    else if (Input.GetKeyDown(KeyCode.UpArrow))
    //        inputDirection = transform.up;
    //    else if (Input.GetKeyDown(KeyCode.DownArrow))
    //        inputDirection = -transform.up;
    //    else
    //        inputDirection = Vector2.zero;
    //}

    /// <summary>
    /// According to player's input to determine how will player move.
    /// </summary>
    private void HandleInput()
    {
        // 抓滑鼠位置算方向
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        moveDirection = mouse - (Vector2)transform.position;
        moveDirection.Normalize();
        coroutineShoot = StartCoroutine(Shoot());
        //// 這次移動最遠可以走多少
        //float maxMovableDistance = 0;
        //// 移動方向上障礙物的位置
        //Ground ground = null;
        //// 水平移動
        //if (moveDirection.x != 0)
        //{
        //    // 套用噴射/走路速度
        //    SpeedCoef = moveSpeed;

        //    // 地上，1格。
        //    if (IsOnGround() && IsFrontHasGround())
        //        distanceCoef = distanceDictionary["move"];
        //    // 空中，2格。
        //    else
        //        distanceCoef = distanceDictionary["rocket"];

        //    // 取得最遠移動距離、是否有障礙物與障礙物位置
        //    bool noObstacle = GetNextMovePointDistance(moveDirection, distanceCoef, out maxMovableDistance, ref ground);
        //    // 可以走至少1格
        //    if (maxMovableDistance != 0)
        //    {
        //        // 紀錄移動方向
        //        oldMoveVector = moveDirection * maxMovableDistance * Constants.moveUnit;
        //        // 更新目的地點
        //        movePoint += oldMoveVector;
        //    }

        //    if (distanceCoef == distanceDictionary["move"])
        //    {
        //        OnWalk?.Invoke(moveDirection);
        //    }
        //    else if (distanceCoef == distanceDictionary["rocket"])
        //    {
        //        firstTimeMiss = true;
        //        OnFireBag?.Invoke(moveDirection);
        //    }
        //    // 會撞牆，演示撞牆後回到正確位置
        //    if (!noObstacle)
        //    {
        //        if (coroutineHitObstacle != null)
        //            StopCoroutine(coroutineHitObstacle);
        //        coroutineHitObstacle = StartCoroutine(HitObstacle(new Vector2(moveDirection.x, 0), ground));
        //        oldMoveVector = distanceCoef * Constants.moveUnit * moveDirection;
        //    }
        //    // 不會撞牆，正常移動
        //    else
        //    {
        //        //Debug.Log("Can Move");
        //        coroutineMovePlayer = StartCoroutine(Shoot());
        //    }

        //}
        //else
        //// 垂直移動
        //if (moveDirection.y != 0)
        //{
        //    // 套用噴射/走路速度
        //    SpeedCoef = moveSpeed;
        //    // 上下2格
        //    distanceCoef = distanceDictionary["rocket"];
        //    firstTimeMiss = true;
        //    // 取得最遠移動距離、是否有障礙物與障礙物位置
        //    bool noObstacle = GetNextMovePointDistance(moveDirection, distanceCoef, out maxMovableDistance, ref ground);
        //    // 可以走至少1格
        //    if (maxMovableDistance != 0)
        //    {
        //        // 紀錄移動方向
        //        oldMoveVector = moveDirection * maxMovableDistance * Constants.moveUnit;
        //        // 更新目的地點
        //        movePoint += oldMoveVector;
        //    }

        //    OnFireBag?.Invoke(moveDirection);

        //    // 會撞牆，演示撞牆後回到正確位置
        //    if (!noObstacle)
        //    {
        //        if (coroutineHitObstacle != null)
        //            StopCoroutine(coroutineHitObstacle);
        //        coroutineHitObstacle = StartCoroutine(HitObstacle(moveDirection, ground));
        //        oldMoveVector = distanceCoef * Constants.moveUnit * moveDirection;
        //    }
        //    // 不會撞牆，正常移動
        //    else
        //        coroutineMovePlayer = StartCoroutine(Shoot());
        //}
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
    /// Force move 1 unit when timemiss or error tempo.
    /// </summary>
    //private void Slide()
    //{
    //    // 只有第一次空拍或打錯才會移動一格
    //    if (!firstTimeMiss)
    //        return;
    //    // 套用滑行速度
    //    SpeedCoef = slideSpeed;

    //    Ground ground = null;
    //    // 取得最遠移動距離、是否有障礙物與障礙物位置
    //    //bool noObstacle = GetNextMovePointDistance(oldMoveVector.normalized, Constants.moveUnit, out float maxDistanceCoef, ref ground, true);
    //    //if (noObstacle)
    //    //    movePoint += oldMoveVector.normalized * Constants.moveUnit * maxDistanceCoef;
    //    //// 會撞牆，演示撞牆後回到正確位置
    //    //else
    //    //{
    //    //    if (coroutineHitObstacle != null)
    //    //        StopCoroutine(coroutineHitObstacle);
    //    //    coroutineHitObstacle = StartCoroutine(HitObstacle(oldMoveVector.normalized, ground));
    //    //}
    //    //firstTimeMiss = false;
    //}

    /// <summary>
    /// Let player move to move point.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Shoot()
    {
        // 移動中，不可操作
        canInput = false;
        // 預先設置目的地
        movePoint = (Vector2)transform.position + moveDirection * distanceDictionary["rocket"];
        SpeedCoef = moveSpeed;
        // 停止滑行
        if (coroutineSlide != null)
            StopCoroutine(coroutineSlide);
        // 直到到達目的地為止
        while (Vector2.Distance(transform.position, movePoint) > SpeedCoef * Time.deltaTime)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint, SpeedCoef * Time.deltaTime);
            yield return null;
        }
        transform.position = movePoint;
        canInput = true;
        coroutineSlide = StartCoroutine(Slide());
    }

    private IEnumerator Slide()
    {
        SpeedCoef = slideSpeed;
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + (Vector3)moveDirection, SpeedCoef * Time.deltaTime);
            movePoint = transform.position;
            yield return null;
        }
    }

    /// <summary>
    /// Show hit obstacle and return to original grid.
    /// </summary>
    /// <param name="direction">Where to go.</param>
    /// <param name="ground">Obstacle's position.</param>
    /// <returns></returns>
    private IEnumerator HitObstacle(Vector2 direction, Ground ground, bool byKnocked = false)
    {
        yield return null;
        //canInput = false;
        //if (coroutineShoot != null)
        //    StopCoroutine(coroutineShoot);
        //int index;
        //// Get hitJudgmentPoints inedx.
        //if (direction.x != 0)
        //{
        //    if (!byKnocked)
        //        index = 3;
        //    else
        //        index = (direction.x == -1) ? 2 : 3;
        //}
        //else
        //    index = (direction.y == 1) ? 0 : 1;
        ////Debug.Log(transform.position - ground.transform.position);
        //GroundEvent groundEvent = ground.GetGroundEvent(transform.position - ground.transform.position);
        //Vector2 obstaclePoint = ground.GetBoundPoint(transform.position - ground.transform.position);
        ////Debug.Log(groundEvent);
        //if (groundEvent != null)
        //{
        //    switch (groundEvent.behavior)
        //    {
        //        case GroundBehavior.None:
        //            // Hit obstacle.
        //            while (Vector2.Distance(hitJudgmentPoints[index].position, obstaclePoint) > 0.02f * Time.deltaTime)
        //            {
        //                Vector2 delta = Vector2.MoveTowards(hitJudgmentPoints[index].position, obstaclePoint, SpeedCoef * Time.deltaTime) - (Vector2)hitJudgmentPoints[index].position;
        //                transform.position += (Vector3)delta;
        //                yield return null;
        //            }
        //            // Bounce.
        //            while (Vector2.Distance(transform.position, movePoint) > 0.02f * Time.deltaTime)
        //            {
        //                transform.position = Vector3.MoveTowards(transform.position, movePoint, SpeedCoef * Time.deltaTime);
        //                yield return null;
        //            }
        //            break;
        //        case GroundBehavior.Standable:
        //            //if (groundEvent.needHit)
        //            //{
        //            //    // Hit obstacle.
        //            //    while (Vector2.Distance(hitJudgmentPoints[index].position, obstaclePoint) > 0.02f * Time.deltaTime)
        //            //    {
        //            //        Vector2 delta = Vector2.MoveTowards(hitJudgmentPoints[index].position, obstaclePoint, SpeedCoef * Time.deltaTime) - (Vector2)hitJudgmentPoints[index].position;
        //            //        transform.position += (Vector3)delta;
        //            //        yield return null;
        //            //    }
        //            //    // Bounce.
        //            //    while (Vector2.Distance(transform.position, movePoint) > 0.02f * Time.deltaTime)
        //            //    {
        //            //        transform.position = Vector3.MoveTowards(transform.position, movePoint, SpeedCoef * Time.deltaTime);
        //            //        yield return null;
        //            //    }
        //            //}
        //            Quaternion q = Quaternion.FromToRotation(-transform.up, moveDirection);
        //            if (q.eulerAngles.z != 0)
        //            {
        //                q.eulerAngles = new Vector3(0, 0, (transform.rotation.eulerAngles.z + q.eulerAngles.z) % 360);
        //                while (Vector3.Distance(transform.rotation.eulerAngles, q.eulerAngles) > 5.625f * Time.deltaTime)
        //                {
        //                    if (q.eulerAngles.z > 0)
        //                        transform.Rotate(Vector3.forward, 5.625f);
        //                    else
        //                        transform.Rotate(-Vector3.forward, 5.625f);
        //                    yield return null;
        //                }
        //                transform.rotation = q;
        //            }
        //            break;
        //        case GroundBehavior.Rebounce:
        //            // Hit obstacle.
        //            while (Vector2.Distance(hitJudgmentPoints[index].position, ground.transform.position) > 0.02f * Time.deltaTime)
        //            {
        //                Vector2 delta = Vector2.MoveTowards(hitJudgmentPoints[index].position, ground.transform.position, SpeedCoef * Time.deltaTime) - (Vector2)hitJudgmentPoints[index].position;
        //                transform.position += (Vector3)delta;
        //                yield return null;
        //            }
        //            //Debug.Log(groundEvent.reboundDirection);
        //            switch (groundEvent.reboundDirection)
        //            {
        //                case GameData.Direction.UP:
        //                    movePoint = (Vector2)ground.transform.position + Vector2.up;
        //                    break;
        //                case GameData.Direction.DOWN:
        //                    movePoint = (Vector2)ground.transform.position + Vector2.down;
        //                    break;
        //                case GameData.Direction.LEFT:
        //                    movePoint = (Vector2)ground.transform.position + Vector2.left;
        //                    break;
        //                case GameData.Direction.RIGHT:
        //                    movePoint = (Vector2)ground.transform.position + Vector2.right;
        //                    break;
        //            }
        //            // Bounce.
        //            while (Vector2.Distance(transform.position, movePoint) > 0.02f * Time.deltaTime)
        //            {
        //                transform.position = Vector3.MoveTowards(transform.position, movePoint, SpeedCoef * Time.deltaTime);
        //                yield return null;
        //            }
        //            break;
        //    }
        //}
        //else
        //{
        //    // Hit obstacle.
        //    while (Vector2.Distance(hitJudgmentPoints[index].position, obstaclePoint) > 0.02f * Time.deltaTime)
        //    {
        //        Vector2 delta = Vector2.MoveTowards(hitJudgmentPoints[index].position, obstaclePoint, SpeedCoef * Time.deltaTime) - (Vector2)hitJudgmentPoints[index].position;
        //        transform.position += (Vector3)delta;
        //        yield return null;
        //    }
        //    // Bounce.
        //    while (Vector2.Distance(transform.position, movePoint) > 0.02f * Time.deltaTime)
        //    {
        //        transform.position = Vector3.MoveTowards(transform.position, movePoint, SpeedCoef * Time.deltaTime);
        //        yield return null;
        //    }
        //}
        //canInput = true;
    }

    /// <summary>
    /// Get next move point distance and check if will hit obstacle.
    /// </summary>
    /// <param name="direction">Where to go.</param>
    /// <param name="distanceFactor">How long to go.</param>
    /// <param name="maxDistance">Max distance can go.</param>
    /// <param name="groundPosition">Obstacke position.</param>
    /// <param name="isSlide">If is slide, no need to detect edge or air.</param>
    /// <returns>True if wont't hit obstacle.</returns>
    private bool GetNextMovePointDistance(Vector2 direction, float distanceFactor, out float maxDistance, ref Ground groundPosition, bool isSlide = false)
    {
        RaycastHit2D hit;
        // 確認移動方向是否有障礙物
        hit = Physics2D.Raycast(movePoint, direction, distanceFactor * Constants.moveUnit, obstacleLayers);
        Debug.DrawLine(movePoint, (Vector2)movePoint + direction * distanceFactor * Constants.moveUnit, Color.green, 1);
        if (hit.collider == null)
        {
            maxDistance = distanceFactor;
            return true;
        }
        maxDistance = Mathf.Floor(hit.distance / Constants.moveUnit);
        groundPosition = hit.collider.GetComponent<Ground>();
        return false;
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
        StartCoroutine(Slide());
    }

    /// <summary>
    /// Force knock player.
    /// </summary>
    /// <param name="direction">推或吸的方向.</param>
    /// <param name="impactFactor">推動或吸動幾個單位.</param>
    /// <param name="impactSpeed">推動或吸動速度.</param>
    public void Knock(Vector2 direction, float impactFactor, float impactSpeed)
    {
        canInput = false;
        // Stop all movement.
        if (coroutineHitObstacle != null)
            StopCoroutine(coroutineHitObstacle);
        if (coroutineShoot != null)
            StopCoroutine(coroutineShoot);
        // Check to determine where player's position is.
        float d = Mathf.Round(Vector2.Distance(transform.position, movePoint) / Constants.moveUnit);
        Vector2 delta = ((Vector2)transform.position - movePoint).normalized;
        movePoint = movePoint + delta * d;
        movePoint = new Vector2(Mathf.Floor(movePoint.x) + 0.5f, Mathf.Floor(movePoint.y) + 0.5f);
        Ground ground = null;
        bool noObstacle = GetNextMovePointDistance(direction, impactFactor, out float maxDistance, ref ground, true);
        movePoint += maxDistance * Constants.moveUnit * direction;
        // No punishment anymore.
        oldMoveVector = Vector2.zero;
        SpeedCoef = impactSpeed;
        // 會撞牆，演示撞牆後回到正確位置
        if (!noObstacle)
            coroutineHitObstacle = StartCoroutine(HitObstacle(direction, ground, true));
        else
            coroutineShoot = StartCoroutine(Shoot());
    }

    /// <summary>
    /// For black hole. Perform fall into black hold effect and
    /// </summary>
    /// <param name="entrance"></param>
    public void FallIntoBlackHole(BlackHole entrance)
    {
        if (isBlackHole)
            return;
        isBlackHole = true;
        canInput = false;
        oldMoveVector = Vector2.zero;
        // Stop all movement.
        if (coroutineHitObstacle != null)
            StopCoroutine(coroutineHitObstacle);
        if (coroutineShoot != null)
            StopCoroutine(coroutineShoot);

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
        canInput = false;
        oldMoveVector = Vector2.zero;
        StartCoroutine(DisplayTeleportIn(entrance));
    }

    /// <summary>
    /// Telepot player out exit.
    /// </summary>
    /// <param name="exit">Exit.</param>
    public void TeleportOut(Teleporter exit)
    {
        canInput = false;
        oldMoveVector = Vector2.zero;
        StartCoroutine(DisplayTeleportOut(exit));
    }

    private IEnumerator DisplayTeleportIn(Teleporter entrance)
    {
        while (Vector2.Distance(transform.position, movePoint) >= SpeedCoef * Time.deltaTime)
            yield return null;
        if (coroutineHitObstacle != null)
            yield return coroutineHitObstacle;
        if (coroutineShoot != null)
            StopCoroutine(coroutineShoot);
        // Rotate and become smaller then disappear.
        while (transform.localScale.magnitude > 10f * Time.deltaTime)
        {
            transform.Rotate(Vector3.forward, entrance.impactRotationSpeed * Time.deltaTime);
            transform.localScale = Vector2.MoveTowards(transform.localScale, Vector2.zero, 10 * Time.deltaTime);
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
        if (coroutineShoot != null)
            StopCoroutine(coroutineShoot);
        coroutineShoot = StartCoroutine(Shoot());
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public void Die()
    {
        if (coroutineShoot != null)
            StopCoroutine(coroutineShoot);
        canInput = false;
        isBlackHole = false;
        firstTimeMiss = true;
    }

    public void StandOnGround(Vector2 direction)
    {
        StopMove();
        Debug.Log("Stop3");
        StartCoroutine("ShowStandOnGround", direction);
    }

    public IEnumerator ShowStandOnGround(Vector2 direction)
    {
        Debug.Log("Start");
        Quaternion q = Quaternion.FromToRotation(-transform.up, direction);
        Debug.Log(q);
        if (q.eulerAngles.z != 0)
        {
            q.eulerAngles = new Vector3(0, 0, (transform.rotation.eulerAngles.z + q.eulerAngles.z) % 360);
            Debug.Log("Start2");
            while (Vector3.Distance(transform.rotation.eulerAngles, q.eulerAngles) > 5.625f * Time.deltaTime)
            {
                Debug.Log("Start3");
                if (q.eulerAngles.z > 0)
                    transform.Rotate(Vector3.forward, 5.625f);
                else
                    transform.Rotate(-Vector3.forward, 5.625f);
                yield return null;
            }
            Debug.Log("Start4");
            transform.rotation = q;
        }
    }

    public void StopMove()
    {
        Debug.Log("Stop1");
        if (coroutineShoot != null)
            StopCoroutine(coroutineShoot);
        if (coroutineSlide != null)
            StopCoroutine(coroutineSlide);
        StopCoroutine("ShowStandOnGround");
        SpeedCoef = 0;
        movePoint = transform.position;
        canInput = true;
        Debug.Log("Stop2");
    }
}
