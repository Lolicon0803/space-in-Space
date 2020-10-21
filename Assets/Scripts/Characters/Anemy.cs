using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Characters;



public class Anemy : MonoBehaviour, IObjectBehavier
{
    public Rigidbody2D rigid;

    public GameData.RouteData[] route;

    public GameData.BulletData bulletData;

    public Bullet bulletPrefab;

    private Vector2 startPoint;

    // 下一個移動點
    private Vector2 movePoint;

    // 移動方向
    private Vector2 moveDiraction;


    void Awake()
    {
        transform.position = new Vector2(Mathf.Floor(transform.position.x) + 0.5f, Mathf.Floor(transform.position.y) + 0.5f);
        startPoint = transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        movePoint = transform.position;
        StartCoroutine("Move");
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            Debug.Log("敵人撞到 扣血");

            // Call損血系統(bool 扣多少血)
        }
    }

    public IEnumerator Move()
    {
        while (true)
        {
            CreateBullet();

            foreach (GameData.RouteData item in route)
            {
                moveDiraction = GameData.Map.directionMap[(int)item.direction];

                for (int i = 0; i < item.distance; i++)
                {
                    // 下個移動點+朝路徑移動1格vector
                    movePoint = (Vector2)transform.position + moveDiraction;

                    while (Vector2.Distance(transform.position, movePoint) > 5 * Time.deltaTime)
                    {
                        transform.position = (Vector3)Vector2.MoveTowards(transform.position, movePoint, 5f * Time.deltaTime);

                        yield return null;
                    }

                    transform.position = movePoint;

                    // Todo:接節奏API
                    yield return new WaitForSeconds(1);

                }
            }
        }

    }

    public void CreateBullet()
    {
        Bullet bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.Set(bulletData);
    }

}
