using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Characters;

public class MovablePlatform : MonoBehaviour, IObjectBehavier
{
    //使用者輸入用
    public GameData.RouteData[] route;
    //迴圈讀取用
    private List<GameData.Direction> routeMap = new List<GameData.Direction>();
    public TempoActionType tempoType;
    public float moveSpeed;

    // 下一個移動點
    private Vector2 movePoint;
    // 移動方向
    private Vector2 moveDirection;
    private int routeIndex = 0;

    private bool hitPlayer;
    private float distance;

    private void Awake()
    {
        hitPlayer = false;
        distance = 0;

        transform.position = new Vector2(Mathf.Floor(transform.position.x) + 0.5f, Mathf.Floor(transform.position.y) + 0.5f);
        //獲得路線
        foreach (GameData.RouteData item in route)
        {
            for (int i = 0; i < item.distance; i++)
            {
                routeMap.Add(item.direction);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        movePoint = transform.position;
        ObjectTempoControl.Singleton.AddToBeatAction(CanMove, tempoType);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CanMove()
    {
        StartCoroutine("Move");
    }

    public IEnumerator Move()
    {
        //確認方向
        moveDirection = GameData.Map.directionMap[(int)routeMap[routeIndex]];

        // 下個移動點+朝路徑移動1格vector
        movePoint = (Vector2)transform.position + moveDirection;

        //移動
        while (Vector2.Distance(transform.position, movePoint) > moveSpeed * Time.deltaTime)
        {
            transform.position = Vector2.MoveTowards(transform.position, movePoint, (float)moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = movePoint;
        routeIndex = (routeIndex + 1) % routeMap.Count;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            //hitPlayer = true;
            //distance = Vector2.Distance(transform.position, Player.Singleton.transform.position);
            Player.Singleton.movement.StopMove();
        }
    }

    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    if (hitPlayer)
    //    {
    //        //Player.Singleton.transform.position += (Vector3)moveDirection * moveSpeed * Time.deltaTime;
    //        //Player.Singleton.movement.Knock(moveDirection, 1, moveSpeed * Time.deltaTime);
    //    }
    //}

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    hitPlayer = false;
    //}
}