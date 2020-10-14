using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private readonly float movementUnit = 1.0f;
    // 是否可以操作
    private bool canInput = false;
    // 是否要滑行(懲罰)
    private bool isSlide = false;

    private Vector2 oldMoveVector;
    private float distanceCoef = 0f;
    private float speedCoef = 1f;

    private Coroutine coroutineHitObstacle;
    private Coroutine coroutineMovePlayer;

    // 所有能阻擋玩家的層(玩家碰撞後回到格子中間)
    public LayerMask obstacleLayers;

    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;
        coroutineMovePlayer = StartCoroutine(MovePlayer());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Knock(Vector2.left, 5.0f, 1.5f);
        if (Input.GetKeyDown(KeyCode.Q))
            Knock(Vector2.right, 5.0f, 1.5f);

        if (canInput)
            return;
        if (Vector2.Distance(transform.position, movePoint.position) <= 0.2)
        {
            float maxDistanceCoef = 0;
            Vector2 obstaclePoint = Vector2.zero;
            int x = 0;
            int y = 0;
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                x = -1;
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                x = 1;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                y = 1;
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                y = -1;
            // float x = Input.GetAxisRaw("Horizontal");
            // float y = Input.GetAxisRaw("Vertical");
            // 水平
            if (x != 0)
            {
                speedCoef = moveSpeed;
                // 是否按下空白鍵 Fix me:操控反直覺
                if (Input.GetKey(KeyCode.Space))
                {
                    distanceCoef = distanceDictionary["rocket"];
                    isSlide = true;
                }
                else
                {
                    distanceCoef = distanceDictionary["move"];
                    isSlide = false;
                }
                bool yes = GetNextMovePointDistance(x, 0, distanceCoef, out maxDistanceCoef, ref obstaclePoint);
                oldMoveVector = new Vector2(maxDistanceCoef * movementUnit * x, 0);
                movePoint.position += (Vector3)oldMoveVector;
                // 會撞牆，演示撞牆後回到正確位置
                if (!yes)
                {
                    if (coroutineHitObstacle != null)
                        StopCoroutine(coroutineHitObstacle);
                    coroutineHitObstacle = StartCoroutine(HitObstacle(new Vector2(x, 0), obstaclePoint));
                    oldMoveVector.x = distanceCoef * movementUnit * x;
                }
            }
            else
            // 垂直 
            if (y != 0)
            {
                speedCoef = moveSpeed;
                distanceCoef = distanceDictionary["rocket"];
                isSlide = true;
                bool yes = GetNextMovePointDistance(0, y, distanceCoef, out maxDistanceCoef, ref obstaclePoint);
                oldMoveVector = new Vector2(0, maxDistanceCoef * movementUnit * y);
                movePoint.position += (Vector3)oldMoveVector;
                // 會撞牆，演示撞牆後回到正確位置
                if (!yes)
                {
                    if (coroutineHitObstacle != null)
                        StopCoroutine(coroutineHitObstacle);
                    coroutineHitObstacle = StartCoroutine(HitObstacle(new Vector2(0, y), obstaclePoint));
                    oldMoveVector.y = distanceCoef * movementUnit * y;
                }
            }
            else
            // 自然滑行一格
            if (isSlide)
            {
                speedCoef = slideSpeed;
                isSlide = false;
                bool yes = GetNextMovePointDistance(oldMoveVector.x / 2, oldMoveVector.y / 2, movementUnit, out maxDistanceCoef, ref obstaclePoint, true);
                if (yes)
                    movePoint.position += (Vector3)oldMoveVector.normalized * movementUnit * maxDistanceCoef;
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
        canInput = true;
        StopCoroutine(coroutineMovePlayer);
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        int index;
        if (direction.x != 0)
            index = (direction.x == -1) ? 2 : 3;
        else
            index = (direction.y == 1) ? 0 : 1;
        while (Vector2.Distance(hitJudgmentPoints[index].position, obstaclePoint) > 0.02f * Time.deltaTime)
        {
            Vector2 delta = Vector2.MoveTowards(hitJudgmentPoints[index].position, obstaclePoint, speedCoef * Time.deltaTime) - (Vector2)hitJudgmentPoints[index].position;
            transform.position += (Vector3)delta;
            yield return null;
        }
        while (Vector2.Distance(transform.position, movePoint.position) > 0.02f * Time.deltaTime)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speedCoef * Time.deltaTime);
            yield return null;
        }
        coroutineMovePlayer = StartCoroutine(MovePlayer());
        canInput = false;
    }

    /// <summary>
    /// Get next move point distance and check if will hit obstacle.
    /// </summary>
    /// <param name="nextX">X that want to go.</param>
    /// <param name="nextY">Y that want to go.</param>
    /// <param name="distanceFactor">How long to go.</param>
    /// <param name="maxDistance">Max distance can go.</param>
    /// <param name="obstaclePosition">Obstacke position.</param>
    /// <returns>True if will hit obstacle.</returns>
    private bool GetNextMovePointDistance(float nextX, float nextY, float distanceFactor, out float maxDistance, ref Vector2 obstaclePosition, bool isSlide = false)
    {
        Vector2 target = new Vector2(nextX, nextY);
        RaycastHit2D hit;

        // 平台邊緣與空中禁止移動1單位
        if ((Mathf.Abs(distanceFactor - movementUnit) <= Mathf.Epsilon) && nextX != 0 && !isSlide)
        {
            // 地圖邊緣
            Collider2D c1 = Physics2D.Raycast(movePoint.position + new Vector3(movementUnit * nextX, 0), Vector2.down, distanceFactor * movementUnit, obstacleLayers).collider;
            // 空中
            Collider2D c2 = Physics2D.Raycast(movePoint.position, Vector2.down, distanceFactor * movementUnit, obstacleLayers).collider;
            if (c1 == null || c2 == null)
            {
                maxDistance = 0;
                return true;
            }
        }
        // 確認移動方向是否有障礙物
        hit = Physics2D.Raycast(movePoint.position, target, distanceFactor * movementUnit, obstacleLayers);
        Debug.DrawLine(movePoint.position, (Vector2)movePoint.position + target * distanceFactor * movementUnit, Color.green, 1);
        if (hit.collider == null)
        {
            maxDistance = distanceFactor;
            return true;
        }
        maxDistance = Mathf.Floor(hit.distance / movementUnit);
        obstaclePosition = hit.point;
        return false;
    }

    /// <summary>
    /// Force knock player.
    /// </summary>
    /// <param name="direction">推的方向.</param>
    /// <param name="impactFactor">推動幾個單位.</param>
    /// <param name="impactSpeed">推動速度.</param>
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
        float d = Mathf.Round(Vector2.Distance(transform.position, movePoint.position) / movementUnit);
        Vector2 delta = (transform.position - movePoint.position).normalized;
        movePoint.position = (Vector2)movePoint.position + delta * d;
        Vector2 obstaclePosition = Vector2.zero;
        bool yes = GetNextMovePointDistance(direction.x, direction.y, impactFactor, out float maxDistance, ref obstaclePosition, true);
        movePoint.position += (Vector3)(maxDistance * movementUnit * direction);
        // No punishment anymore.
        oldMoveVector = Vector2.zero;
        speedCoef = impactSpeed;
        // 會撞牆，演示撞牆後回到正確位置
        if (!yes)
            coroutineHitObstacle = StartCoroutine(HitObstacle(direction, obstaclePosition));
        else
            coroutineMovePlayer = StartCoroutine(MovePlayer());
    }
}
