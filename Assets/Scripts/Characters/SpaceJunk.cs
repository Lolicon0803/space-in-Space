using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Characters;


public class SpaceJunk : MonoBehaviour, IObjectBehavier
{
    public Rigidbody2D rigid;

    //使用者輸入用
    public GameData.RouteData[] route;

    //迴圈讀取用
    private List<GameData.Direction> routeMap = new List<GameData.Direction>();

    public TempoActionType tempoType;

    public float knockDistance;

    public float knockPower;

    public float moveSpeed;

    public bool isGoStartPoint;

   
    //初始位置
    private Vector2 startPoint;

    // 下一個移動點
    private Vector2 movePoint;

    // 移動方向
    private Vector2 moveDiraction;

    private int routeIndex = 0;

    void Awake()
    {
        transform.position = new Vector2(Mathf.Floor(transform.position.x) + 0.5f, Mathf.Floor(transform.position.y) + 0.5f);
        startPoint = transform.position;


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

    public IEnumerator Move()
    {
      


        // 回到初始位置判定
        if (routeIndex % routeMap.Count == 0 && isGoStartPoint)
        {
            transform.position = startPoint;
        }

        // 確認下個移動方向
        moveDiraction = GameData.Map.directionMap[(int)routeMap[routeIndex]];

        // 下個移動點+朝路徑移動1格vector
        movePoint = (Vector2)transform.position + moveDiraction;

        /**
         * 往自己下個移動點的位置註冊撞擊
         */
        MapSystem.Singleton.SetMapEvent(movePoint, EventList.Hit, true,Vector2.right, (short)knockDistance, (short)knockPower);


        // 移動
        while (Vector2.Distance(transform.position, movePoint) > moveSpeed * Time.deltaTime)
        {
            transform.position = (Vector3)Vector2.MoveTowards(transform.position, movePoint, (float)moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = movePoint;
        routeIndex = (routeIndex + 1) % routeMap.Count;
    }

    void CanMove()
    {
        StartCoroutine("Move");
    }

}
