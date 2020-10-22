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

    // 移動單位
    //private readonly float Constants.moveUnit = 1.0f;

    // 是否可以操作
    private bool canInput = true;

    // 是否要滑行(懲罰)
    private bool mayPunish = false;

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
    private bool spacePressed;

    // Start is called before the first frame update
    void Start()
    {
        SpeedCoef = 1.0f;
        movePoint.parent = null;
        coroutineMovePlayer = StartCoroutine(MovePlayer());
        /*
        FindObjectOfType<AudioEngine>().SetTempoTypeListener(() =>
        {
            Punish();
            OnMiss?.Invoke();
        }, TempoActionType.TimeOut);*/
        //FindObjectOfType<AudioEngine>().SetTempoTypeListener(() => { }, TempoActionType.TimeOut);
       // FindObjectOfType<AudioEngine>().SetTempoTypeListener(() => { }, TempoActionType.Quarter);
      //  FindObjectOfType<AudioEngine>().SetTempoTypeListener(() => { }, TempoActionType.Half);
       // FindObjectOfType<AudioEngine>().SetTempoTypeListener(() => { }, TempoActionType.Whole);
        //FindObjectOfType<AudioEngine>().BPM = 60;
        inputDirection = Vector2.zero;
        spacePressed = false;
        StartCoroutine(ProcessOperation());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator ProcessOperation()
    {
        while (true)
        {
            if (canInput)
            {
                if (Vector2.Distance(transform.position, movePoint.position) <= SpeedCoef * Time.deltaTime)
                {
                    yield return StartCoroutine(CheckInput());
                    if (inputDirection != Vector2.zero || spacePressed)
                    {
                        if (FindObjectOfType<AudioEngine>().KeyDown())
                            HandleInput(inputDirection, spacePressed);
                        else
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
    private IEnumerator CheckInput()
    {
        float t = 0;
        inputDirection = Vector2.zero;
        spacePressed = false;
        // 讓玩家在x幀內都能輸入，不然同一幀有時候未必能偵測到空白鍵+左右鍵
        while (t < Time.deltaTime * 10.0f)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                inputDirection = Vector2.left;
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                inputDirection = Vector2.right;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                inputDirection = Vector2.up;
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                inputDirection = Vector2.down;
            if (Input.GetKeyDown(KeyCode.Space))
                spacePressed = true;
            yield return null;
            t += Time.deltaTime;
        }
        yield return null;
    }

    /// <summary>
    /// According to player's input to determine how will player move.
    /// </summary>
    /// <param name="direction">Player's input direction.</param>
    /// <param name="spacePressed">Is space pressed.</param>
    private void HandleInput(Vector2 direction, bool spacePressed)
    {
        float maxDistanceCoef = 0;
        Vector2 obstaclePoint = Vector2.zero;
        // 水平
        if (direction.x != 0)
        {
            mayPunish = true;
            SpeedCoef = moveSpeed;

            if (spacePressed)
                distanceCoef = distanceDictionary["rocket"];
            else
                distanceCoef = distanceDictionary["move"];

            bool noObstacle = GetNextMovePointDistance(direction, distanceCoef, out maxDistanceCoef, ref obstaclePoint);

            if (maxDistanceCoef != 0)
            {
                oldMoveVector = direction * maxDistanceCoef * Constants.moveUnit;
                movePoint.position += (Vector3)oldMoveVector;
            }

            if (distanceCoef == distanceDictionary["move"] && (maxDistanceCoef != 0 || (!noObstacle && maxDistanceCoef == 0)))
            {
                OnWalk?.Invoke(direction);
                mayPunish = false;
            }
            else if (distanceCoef == distanceDictionary["rocket"])
            {
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
            mayPunish = true;
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

    /// <summary>
    /// Force move 1 unit when timemiss or error tempo.
    /// </summary>
    private void Punish()
    {
        if (!mayPunish)
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

        // 平台邊緣與空中禁止移動1單位
        if ((Mathf.Abs(distanceFactor - Constants.moveUnit) <= Mathf.Epsilon) && direction.x != 0 && !isSlide)
        {
            // 地圖邊緣
            Collider2D c1 = Physics2D.Raycast(movePoint.position + new Vector3(Constants.moveUnit * direction.x, 0), Vector2.down, distanceFactor * Constants.moveUnit, obstacleLayers).collider;
            // 空中
            Collider2D c2 = Physics2D.Raycast(movePoint.position, Vector2.down, distanceFactor * Constants.moveUnit, obstacleLayers).collider;
            if (c1 == null || c2 == null)
            {
                maxDistance = 0;
                return true;
            }
        }
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
        mayPunish = false;
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
        mayPunish = false;
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

        // 死亡，回起點。

        //movePoint.position = exit.transform.position;
        //transform.position = exit.transform.position;
        //// Wait.
        //yield return StartCoroutine(entrance.WaitToExit());
        //// Rotate, appear and become bigger.
        //while (transform.localScale.magnitude < Vector2.one.magnitude)
        //{
        //    transform.Rotate(Vector3.forward, exit.pushRotationSpeed * Time.deltaTime);
        //    transform.localScale = Vector2.MoveTowards(transform.localScale, Vector2.one, 1 * Time.deltaTime);
        //    yield return null;
        //}
        //transform.localScale = Vector2.one;
        //transform.rotation = Quaternion.identity;
        //isBlackHole = false;
        //Knock(exit.pushDirection, exit.pushUnit, exit.pushSpeed);
    }

    /// <summary>
    /// Telepot player to exit.
    /// </summary>
    /// <param name="entrance">Entrance.</param>
    /// <param name="exit">Where player to go.</param>
    public void Teleport(Teleporter entrance, Teleporter exit)
    {
        canInput = false;
        mayPunish = false;
        oldMoveVector = Vector2.zero;
        StartCoroutine(DisplayTeleport(entrance, exit));
    }

    private IEnumerator DisplayTeleport(Teleporter entrance, Teleporter exit)
    {
        while (Vector2.Distance(transform.position, movePoint.position) >= SpeedCoef * Time.deltaTime)
            yield return null;
        if (coroutineHitObstacle != null)
            yield return coroutineHitObstacle;
        if (coroutineMovePlayer != null)
            StopCoroutine(coroutineMovePlayer);
        // Rotate and become smaller then disappear.
        while (transform.localScale.magnitude > 0.1f * Time.deltaTime)
        {
            transform.Rotate(Vector3.forward, entrance.impactRotationSpeed * Time.deltaTime);
            transform.localScale = Vector2.MoveTowards(transform.localScale, Vector2.zero, 1 * Time.deltaTime);
            yield return null;
        }
        transform.localScale = Vector2.zero;
        // Wait.
        yield return StartCoroutine(entrance.WaitToExit());
        movePoint.position = exit.transform.position;
        transform.position = exit.transform.position;
        // Rotate, appear and become bigger.
        while (transform.localScale.magnitude < Vector2.one.magnitude)
        {
            transform.Rotate(Vector3.forward, exit.pushRotationSpeed * Time.deltaTime);
            transform.localScale = Vector2.MoveTowards(transform.localScale, Vector2.one, 1 * Time.deltaTime);
            yield return null;
        }
        transform.localScale = Vector2.one;
        transform.rotation = Quaternion.identity;
        Knock(exit.pushDirection, exit.pushUnit, exit.pushSpeed);
    }


    private void TEST()
    {

    }

}
