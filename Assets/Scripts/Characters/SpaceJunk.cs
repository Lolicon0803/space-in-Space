﻿using System.Collections;
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

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            Vector2 dir = Player.Singleton.transform.position - transform.position;
            if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
                dir.y = 0;
            else
                dir.x = 0;
            dir.Normalize();
            if (moveDiraction != dir)
                collider.GetComponent<PlayerMovement>().Knock(Vector2.zero, 1, knockPower);
            else
                collider.GetComponent<PlayerMovement>().Knock(moveDiraction, knockDistance, knockPower);
            Debug.Log("撞到敵人");
            gameObject.GetComponent<AudioSource>().Play();
            // Call損血系統
            Player.Singleton.lifeSystem.Hurt();
        }
    }

    public IEnumerator Move()
    {

        if (routeIndex % routeMap.Count == 0 && isGoStartPoint)
        {
            transform.position = startPoint;
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

    void CanMove()
    {
        StartCoroutine("Move");
    }

}