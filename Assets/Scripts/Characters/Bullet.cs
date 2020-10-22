using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bullet : MonoBehaviour
{
    private Vector2 movePoint;
    private Vector2 moveDiraction;
    private float moveSpeed;
    private float moveDistance;
    public TempoActionType tempoType;
    private int routeIndex = 0;

    public void Set(GameData.BulletData bulletData)
    {
        moveDiraction = GameData.Map.directionMap[(int)bulletData.direction];
        moveSpeed = bulletData.speed;
        moveDistance = bulletData.distance;
        tempoType = bulletData.tempoType;
    }

    void Awake()
    {
        transform.position = new Vector2(Mathf.Floor(transform.position.x) + 0.5f, Mathf.Floor(transform.position.y) + 0.5f);
    }

    // Start is called before the first frame update
    void Start()
    {
        movePoint = transform.position;
        ObjectTempoControl.Singleton.AddToBeatAction(CanMove, tempoType);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            // Call損血系統(bool 扣多少血)
            Debug.Log("子彈命中 扣血");
            //自己消失
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator Move()
    {
        // 下個移動點+朝路徑移動1格vector
        movePoint = (Vector2)transform.position + moveDiraction;

        while (Vector2.Distance(transform.position, movePoint) > moveSpeed * Time.deltaTime)
        {
            transform.position = (Vector3)Vector2.MoveTowards(transform.position, movePoint, moveSpeed * Time.deltaTime);

            yield return null;
        }

        transform.position = movePoint;

        routeIndex = (routeIndex + 1) % (int)moveDistance;

        if (routeIndex % moveDistance == 0)
        {
            routeIndex = 0;
            ObjectTempoControl.Singleton.RemoveToBeatAction(CanMove, tempoType);
            Destroy(gameObject);
        }
    }

    void CanMove()
    {
        StartCoroutine("Move");
    }
}
