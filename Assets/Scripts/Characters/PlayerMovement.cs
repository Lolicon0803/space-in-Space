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
    public Transform movePoint;

    // 四個碰撞點
    public Transform[] hitJudgmentPoints;

    // 是否可以操作
    public bool canInput = true;

    // 是否第一次空拍
    public bool firstTimeMiss;

    // 黑洞中，優先度最高
    private bool isBlackHole = false;

    private Vector2 oldMoveVector;
    public Vector2 OldMoveVector
    {
        get { return oldMoveVector; }
        private set { oldMoveVector = value; }
    }
    private float distanceCoef = 0f;
    public float SpeedCoef { get; private set; }

    private Coroutine coroutineHitObstacle;
    private Coroutine coroutineMovePlayer;

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

    private Vector2 inputDirection;
    //private bool spacePressed;

    // Start is called before the first frame update
    void Start()
    {
        SpeedCoef = 1.0f;
        movePoint.parent = null;

        ObjectTempoControl.Singleton.AddToBeatAction(() =>
        {
            if (!IsOnGround() && canInput)
            {
                OnMiss?.Invoke();
                Punish();
            }
        }, TempoActionType.TimeOut);

        inputDirection = Vector2.zero;
        //spacePressed = false;
        firstTimeMiss = true;

        coroutineMovePlayer = StartCoroutine(MovePlayer());
        StartCoroutine(ProcessOperation());
    }

    private IEnumerator ProcessOperation()
    {
        while (true)
        {
            if (canInput)
            {
                // 已在定點。
                if (Vector2.Distance(transform.position, movePoint.position) <= SpeedCoef * Time.deltaTime)
                {
                    CheckInput();
                    //if (inputDirection != Vector2.zero || spacePressed)
                    if (inputDirection != Vector2.zero)
                    {
                        // 打在節拍上
                        if (AudioEngine.Singleton.KeyDown())
                            HandleInput(inputDirection);
                        // 沒有打在節拍上且不在地上
                        else if (!IsOnGround())
                        {
                            OnError?.Invoke();
                            Punish();
                        }
                    }
                }
            }
            yield return null;
        }
    }

    /// <summary>
    /// Get player's keyboard input.
    /// </summary>
    /// <returns></returns>
    private void CheckInput()
    {
        //float t = 0;
        
        //spacePressed = false;
        // 讓玩家在x幀內都能輸入，不然同一幀有時候未必能偵測到空白鍵+左右鍵
        //while (t < Time.deltaTime * 7.5f)
        //{
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            inputDirection = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            inputDirection = Vector2.right;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            inputDirection = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            inputDirection = Vector2.down;
        else
            inputDirection = Vector2.zero;
        //if (Input.GetKeyDown(KeyCode.Space))
        //    spacePressed = true;
        //yield return null;
        //t += Time.deltaTime;
        //}
        //yield return null;
    }

    /// <summary>
    /// According to player's input to determine how will player move.
    /// </summary>
    /// <param name="direction">Player's input direction.</param>
    /// <param name="spacePressed">Is space pressed.</param>
    private void HandleInput(Vector2 direction)
    {
        float maxDistanceCoef = 0;
        Vector2 obstaclePoint = Vector2.zero;
        // 水平
        if (direction.x != 0)
        {
            SpeedCoef = moveSpeed;

            // 地上，1格。
            if (IsOnGround() && IsFrontHasGround())
                distanceCoef = distanceDictionary["move"];
            // 空中，2格。
            else
                distanceCoef = distanceDictionary["rocket"];

            bool noObstacle = GetNextMovePointDistance(direction, distanceCoef, out maxDistanceCoef, ref obstaclePoint);

            if (maxDistanceCoef != 0)
            {
                oldMoveVector = direction * maxDistanceCoef * Constants.moveUnit;
                movePoint.position += (Vector3)oldMoveVector;
            }

            if (distanceCoef == distanceDictionary["move"])
            {
                OnWalk?.Invoke(direction);
            }
            else if (distanceCoef == distanceDictionary["rocket"])
            {
                firstTimeMiss = true;
                OnFireBag?.Invoke(direction);
            }

            // 會撞牆，演示撞牆後回到正確位置
            if (!noObstacle)
            {
                if (coroutineHitObstacle != null)
                    StopCoroutine(coroutineHitObstacle);
                coroutineHitObstacle = StartCoroutine(HitObstacle(new Vector2(direction.x, 0), obstaclePoint));
                oldMoveVector.x = distanceCoef * Constants.moveUnit * direction.x;
            }
        }
        else
        // 垂直 
        if (direction.y != 0)
        {
            SpeedCoef = moveSpeed;
            distanceCoef = distanceDictionary["rocket"];
            firstTimeMiss = true;
            bool noObstacle = GetNextMovePointDistance(direction, distanceCoef, out maxDistanceCoef, ref obstaclePoint);

            if (maxDistanceCoef != 0)
            {
                oldMoveVector = oldMoveVector = direction * maxDistanceCoef * Constants.moveUnit;
                movePoint.position += (Vector3)oldMoveVector;
            }

            OnFireBag?.Invoke(direction);

            // 會撞牆，演示撞牆後回到正確位置
            if (!noObstacle)
            {
                if (coroutineHitObstacle != null)
                    StopCoroutine(coroutineHitObstacle);
                coroutineHitObstacle = StartCoroutine(HitObstacle(direction, obstaclePoint));
                oldMoveVector.y = distanceCoef * Constants.moveUnit * direction.y;
            }
        }
    }

    private bool IsOnGround()
    {
        // 判斷自己下方是否有地板
        Collider2D c = Physics2D.Raycast(movePoint.position, Vector2.down, Constants.moveUnit, obstacleLayers).collider;
        return c != null;
    }

    private bool IsFrontHasGround()
    {
        // 判斷前方一格下方是否有地板
        Collider2D c = Physics2D.Raycast(movePoint.position + new Vector3(Constants.moveUnit * inputDirection.x, 0), Vector2.down, Constants.moveUnit, obstacleLayers).collider;
        return c != null;
    }

    /// <summary>
    /// Force move 1 unit when timemiss or error tempo.
    /// </summary>
    private void Punish()
    {
        if (!firstTimeMiss)
            return;

        SpeedCoef = slideSpeed;

        Vector2 obstaclePoint = Vector2.zero;

        bool yes = GetNextMovePointDistance(oldMoveVector.normalized, Constants.moveUnit, out float maxDistanceCoef, ref obstaclePoint, true);
        if (yes)
            movePoint.position += (Vector3)oldMoveVector.normalized * Constants.moveUnit * maxDistanceCoef;
        // 會撞牆，演示撞牆後回到正確位置
        else
        {
            if (coroutineHitObstacle != null)
                StopCoroutine(coroutineHitObstacle);
            coroutineHitObstacle = StartCoroutine(HitObstacle(oldMoveVector.normalized, obstaclePoint));
        }
        firstTimeMiss = false;
    }

    /// <summary>
    /// Let player move to move point.
    /// </summary>
    /// <returns></returns>
    private IEnumerator MovePlayer()
    {
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, SpeedCoef * Time.deltaTime);
            yield return null;
        }
    }

    /// <summary>
    /// Show hit obstacle and return to original grid.
    /// </summary>
    /// <param name="direction">Where to go.</param>
    /// <param name="obstaclePoint">Obstacle's position.</param>
    /// <returns></returns>
    private IEnumerator HitObstacle(Vector2 direction, Vector2 obstaclePoint)
    {
        canInput = false;
        StopCoroutine(coroutineMovePlayer);
        int index;
        // Get hitJudgmentPoints inedx.
        if (direction.x != 0)
            index = 3; //  (direction.x == -1) ? 2 : 3;
        else
            index = (direction.y == 1) ? 0 : 1;
        // Hit obstacle.
        while (Vector2.Distance(hitJudgmentPoints[index].position, obstaclePoint) > 0.02f * Time.deltaTime)
        {
            Vector2 delta = Vector2.MoveTowards(hitJudgmentPoints[index].position, obstaclePoint, SpeedCoef * Time.deltaTime) - (Vector2)hitJudgmentPoints[index].position;
            transform.position += (Vector3)delta;
            yield return null;
        }
        // Bounce.
        while (Vector2.Distance(transform.position, movePoint.position) > 0.02f * Time.deltaTime)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, SpeedCoef * Time.deltaTime);
            yield return null;
        }
        coroutineMovePlayer = StartCoroutine(MovePlayer());
        canInput = true;
    }

    /// <summary>
    /// Get next move point distance and check if will hit obstacle.
    /// </summary>
    /// <param name="direction">Where to go.</param>
    /// <param name="distanceFactor">How long to go.</param>
    /// <param name="maxDistance">Max distance can go.</param>
    /// <param name="obstaclePosition">Obstacke position.</param>
    /// <param name="isSlide">If is slide, no need to detect edge or air.</param>
    /// <returns>True if wont't hit obstacle.</returns>
    private bool GetNextMovePointDistance(Vector2 direction, float distanceFactor, out float maxDistance, ref Vector2 obstaclePosition, bool isSlide = false)
    {
        RaycastHit2D hit;
        // 確認移動方向是否有障礙物
        hit = Physics2D.Raycast(movePoint.position, direction, distanceFactor * Constants.moveUnit, obstacleLayers);
        Debug.DrawLine(movePoint.position, (Vector2)movePoint.position + direction * distanceFactor * Constants.moveUnit, Color.green, 1);
        if (hit.collider == null)
        {
            maxDistance = distanceFactor;
            return true;
        }
        maxDistance = Mathf.Floor(hit.distance / Constants.moveUnit);
        obstaclePosition = hit.point;
        return false;
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
        if (coroutineMovePlayer != null)
            StopCoroutine(coroutineMovePlayer);
        // Check to determine where player's position is.
        float d = Mathf.Round(Vector2.Distance(transform.position, movePoint.position) / Constants.moveUnit);
        Vector2 delta = (transform.position - movePoint.position).normalized;
        movePoint.position = (Vector2)movePoint.position + delta * d;
        movePoint.position = new Vector2(Mathf.Floor(movePoint.position.x) + 0.5f, Mathf.Floor(movePoint.position.y) + 0.5f);
        Vector2 obstaclePosition = Vector2.zero;
        bool noObstacle = GetNextMovePointDistance(direction, impactFactor, out float maxDistance, ref obstaclePosition, true);
        movePoint.position += (Vector3)(maxDistance * Constants.moveUnit * direction);
        // No punishment anymore.
        oldMoveVector = Vector2.zero;
        SpeedCoef = impactSpeed;
        // 會撞牆，演示撞牆後回到正確位置
        if (!noObstacle)
            coroutineHitObstacle = StartCoroutine(HitObstacle(direction, obstaclePosition));
        else
        {
            coroutineMovePlayer = StartCoroutine(MovePlayer());
            canInput = true;
        }
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
        if (coroutineMovePlayer != null)
            StopCoroutine(coroutineMovePlayer);

        StartCoroutine(DisplayFallIntoBlackHole(entrance));
    }

    private IEnumerator DisplayFallIntoBlackHole(BlackHole entrance)
    {
        OnFallIntoBlackHole?.Invoke();

        movePoint.position = entrance.transform.position;
        // Rotate and move.
        while (Vector2.Distance(transform.position, movePoint.position) > entrance.impactSpeed * Time.deltaTime)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, entrance.impactSpeed * Time.deltaTime);
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
        while (Vector2.Distance(transform.position, movePoint.position) >= SpeedCoef * Time.deltaTime)
            yield return null;
        if (coroutineHitObstacle != null)
            yield return coroutineHitObstacle;
        if (coroutineMovePlayer != null)
            StopCoroutine(coroutineMovePlayer);
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
        movePoint.position = exit.transform.position;
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
}
