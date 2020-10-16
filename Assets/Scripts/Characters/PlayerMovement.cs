using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Constants
{
    //世界座標移動單位
    public const float moveUnit = 1.0f;

    // 移動速度係數
    public const float moveSpeed = 5f;

    // 滑行速度系數
    public const float slideSpeed = 1f;

}

public class PlayerMovement : MonoBehaviour
{
    // GameData Todo:之後要移動

    // 移動距離係數
    private readonly Dictionary<string, float> distanceDictionary = new Dictionary<string, float> { { "rocket", 2f }, { "move", 1f } };

    // 下一個移動點
    public Transform movePoint;

    // 四個碰撞點
    public Transform[] hitJudgmentPoints;

    // 移動單位
    //private readonly float Constants.moveUnit = 1.0f;

    // 是否可以操作
    private bool canInput = true;

    // 是否要滑行(懲罰)
    private bool isSlide = false;

    // 黑洞中，優先度最高
    private bool isBlackHole = false;

    private Vector2 oldMoveVector;
    private float distanceCoef = 0f;
    private float speedCoef = 1f;

    private Coroutine coroutineHitObstacle;
    private Coroutine coroutineMovePlayer;

    // 所有能阻擋玩家的層(玩家碰撞後回到格子中間)
    public LayerMask obstacleLayers;

    // All player behavier.
    public delegate void PlayerBehavierDelegate();
    public event PlayerBehavierDelegate OnWalk;
    public event PlayerBehavierDelegate OnFireBag;
    public event PlayerBehavierDelegate OnFallIntoBlackHole;

    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;
        coroutineMovePlayer = StartCoroutine(MovePlayer());
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, movePoint.position) <= 0.2 * Time.deltaTime)
        {
            float maxDistanceCoef = 0;
            Vector2 obstaclePoint = Vector2.zero;

            // 判斷方向用座標
            Vector2 direction = Vector2.zero;
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                direction = Vector2.left;
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                direction = Vector2.right;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                direction = Vector2.up;
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                direction = Vector2.down;

            // 水平
            if (direction.x != 0)
            {
                speedCoef = Constants.moveSpeed;

                // 是否按下空白鍵 Fix me:操控反直覺
                if (Input.GetKey(KeyCode.Space))
                {
                    if (OnFireBag != null)
                        OnFireBag.Invoke();

                    distanceCoef = distanceDictionary["rocket"];
                    isSlide = true;
                }
                else
                {
                    if (OnWalk != null)
                        OnWalk.Invoke();

                    distanceCoef = distanceDictionary["move"];
                    isSlide = false;
                }

                bool noObstacle = GetNextMovePointDistance(direction, distanceCoef, out maxDistanceCoef, ref obstaclePoint);

                oldMoveVector = direction * maxDistanceCoef * Constants.moveUnit;
                movePoint.position += (Vector3)oldMoveVector;

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
                speedCoef = Constants.moveSpeed;
                distanceCoef = distanceDictionary["rocket"];
                isSlide = true;

                if (OnFireBag != null)
                    OnFireBag.Invoke();

                bool noObstacle = GetNextMovePointDistance(direction, distanceCoef, out maxDistanceCoef, ref obstaclePoint);

                oldMoveVector = oldMoveVector = direction * maxDistanceCoef * Constants.moveUnit;
                movePoint.position += (Vector3)oldMoveVector;


                // 會撞牆，演示撞牆後回到正確位置
                if (!noObstacle)
                {
                    if (coroutineHitObstacle != null)
                        StopCoroutine(coroutineHitObstacle);
                    coroutineHitObstacle = StartCoroutine(HitObstacle(direction, obstaclePoint));
                    oldMoveVector.y = distanceCoef * Constants.moveUnit * direction.y;
                }
            }
            else
            // 自然滑行一格
            if (isSlide)
            {
                speedCoef = Constants.slideSpeed;
                isSlide = false;

                bool yes = GetNextMovePointDistance(oldMoveVector.normalized, Constants.moveUnit, out maxDistanceCoef, ref obstaclePoint, true);
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
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speedCoef * Time.deltaTime);
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
            index = (direction.x == -1) ? 2 : 3;
        else
            index = (direction.y == 1) ? 0 : 1;
        // Hit obstacle.
        while (Vector2.Distance(hitJudgmentPoints[index].position, obstaclePoint) > 0.02f * Time.deltaTime)
        {
            Vector2 delta = Vector2.MoveTowards(hitJudgmentPoints[index].position, obstaclePoint, speedCoef * Time.deltaTime) - (Vector2)hitJudgmentPoints[index].position;
            transform.position += (Vector3)delta;
            yield return null;
        }
        // Bounce.
        while (Vector2.Distance(transform.position, movePoint.position) > 0.02f * Time.deltaTime)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speedCoef * Time.deltaTime);
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
    /// <returns>True if will hit obstacle.</returns>
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
        isSlide = false;
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
        Vector2 obstaclePosition = Vector2.zero;
        bool noObstacle = GetNextMovePointDistance(direction, impactFactor, out float maxDistance, ref obstaclePosition, true);
        movePoint.position += (Vector3)(maxDistance * Constants.moveUnit * direction);
        // No punishment anymore.
        oldMoveVector = Vector2.zero;
        speedCoef = impactSpeed;
        // 會撞牆，演示撞牆後回到正確位置
        if (!noObstacle)
            coroutineHitObstacle = StartCoroutine(HitObstacle(direction, obstaclePosition));
        else
            coroutineMovePlayer = StartCoroutine(MovePlayer());
    }

    /// <summary>
    /// For black hole. Perform fall into black hold effect and
    /// </summary>
    /// <param name="target"></param>
    /// <param name="speed"></param>
    public void FallIntoBlackHole(BlackHole entrance, BlackHole exit)
    {
        if (isBlackHole)
            return;
        isBlackHole = true;
        canInput = false;
        isSlide = false;
        // Stop all movement.
        if (coroutineHitObstacle != null)
            StopCoroutine(coroutineHitObstacle);
        if (coroutineMovePlayer != null)
            StopCoroutine(coroutineMovePlayer);

        StartCoroutine(DisplayFallIntoBlackHole(entrance, exit));
    }

    private IEnumerator DisplayFallIntoBlackHole(BlackHole entrance, BlackHole exit)
    {
        if (OnFallIntoBlackHole != null)
            OnFallIntoBlackHole.Invoke();
    
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
        movePoint.position = exit.transform.position;
        transform.position = exit.transform.position;
        // Wait.
        yield return StartCoroutine(entrance.WaitToExit());
        // Rotate, appear and become bigger.
        while (transform.localScale.magnitude < Vector2.one.magnitude)
        {
            transform.Rotate(Vector3.forward, exit.pushRotationSpeed * Time.deltaTime);
            transform.localScale = Vector2.MoveTowards(transform.localScale, Vector2.one, 1 * Time.deltaTime);
            yield return null;
        }
        transform.localScale = Vector2.one;
        transform.rotation = Quaternion.identity;
        isBlackHole = false;
        Knock(exit.pushDirection, exit.pushUnit, exit.pushSpeed);
    }
}
