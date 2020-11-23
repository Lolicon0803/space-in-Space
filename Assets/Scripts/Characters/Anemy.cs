using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Characters;



public class Anemy : MonoBehaviour, IObjectBehavier
{
    public Rigidbody2D rigid;

    //使用者輸入用
    public GameData.RouteData[] route;

    //迴圈讀取用
    private List<GameData.Direction> routeMap = new List<GameData.Direction>();

    public TempoActionType tempoType;

    public GameData.BulletData bulletData;

    public Bullet bulletPrefab;

    // 是不是被boss叫的
    public bool fromBoss = false;

    private Vector2 startPoint;

    // 下一個移動點
    private Vector2 movePoint;

    // 移動方向
    private Vector2 moveDiraction;

    public float moveSpeed;

    private int routeIndex = 0;

    public delegate void EnemyEvent();
    public event EnemyEvent OnDisappear;

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
        if (!fromBoss)
            ObjectTempoControl.Singleton.AddToBeatAction(CanMove, tempoType);
    }

    /// <summary>
    /// For boss, let enemy go to destination.
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="speed"></param>
    public void SetDestination(Vector2 destination, float speed)
    {
        StartCoroutine(MoveTo(destination, speed));
    }

    /// <summary>
    /// Go to destination.
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveTo(Vector2 destination, float speed)
    {
        while (Vector2.Distance(transform.position, destination) > speed * Time.deltaTime)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            yield return null;
        }
        ObjectTempoControl.Singleton.AddToBeatAction(CanMove, tempoType);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            Debug.Log("敵人撞到 扣血");
            gameObject.GetComponent<AudioSource>().Play();
            // Call損血系統
            Player.Singleton.lifeSystem.Hurt();
        }
    }

    public IEnumerator Move()
    {
        if (routeMap.Count > 0)
        {
            if (routeIndex % routeMap.Count == 0)
            {
                CreateBullet();
            }
            //確認方向
            moveDiraction = GameData.Map.directionMap[(int)routeMap[routeIndex]];

            // 下個移動點+朝路徑移動1格vector
            movePoint = (Vector2)transform.position + moveDiraction;

            //移動
            while (Vector2.Distance(transform.position, movePoint) > moveSpeed * Time.deltaTime)
            {
                transform.position = (Vector3)Vector2.MoveTowards(transform.position, movePoint, (float)moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = movePoint;
            routeIndex = (routeIndex + 1) % routeMap.Count;
        }
        else
        {
            CreateBullet();
        }
    }

    public void CreateBullet()
    {
        Bullet bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.Set(bulletData);
    }


    void CanMove()
    {
        StartCoroutine("Move");
    }

    public void Disappear()
    {
        ObjectTempoControl.Singleton.RemoveToBeatAction(CanMove, tempoType);
        OnDisappear?.Invoke();
        Destroy(gameObject);
    }

}
