using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermove3 : MonoBehaviour
{
    //GameData Todo:之後要移動

    //移動距離係數
    Dictionary<string, float> distance = new Dictionary<string, float> { { "rocket", 2f }, { "move", 1f } };

    //移動速度係數
    public float moveSpeed = 5f;

    //滑行速度系數
    public float slideSpeed = 1f;

    //下一個移動點
    public Transform movePoint;

    //是否要滑行
    private bool isSlide = false;

    private Vector3 oldMoveVector;
    private float distanceCoef = 0f;
    private float speedCoef = 1f;

    //未完成功能 是否被撞擊
    public bool isKnock = false;


    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {

        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speedCoef * Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) <= 0.2f)
        {
            speedCoef = moveSpeed;

            //水平
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            {
                //是否按下空白鍵 Fix me:操控反直覺
                if (Input.GetButton("Jump"))
                {
                    distanceCoef = distance["rocket"];
                    isSlide = true;
                }
                else
                {
                    distanceCoef = distance["move"];
                    isSlide = false;
                }
                oldMoveVector = new Vector3(distanceCoef * Input.GetAxisRaw("Horizontal"), 0f, 0f);
                movePoint.position += oldMoveVector;
            }
            else
            //垂直 
            if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            {

                distanceCoef = distance["rocket"];
                isSlide = true;

                oldMoveVector = new Vector3(0f, distanceCoef * Input.GetAxisRaw("Vertical"), 0f);
                movePoint.position += oldMoveVector;

            }
            else
            //自然滑行一格
            if (isSlide)
            {
                movePoint.position += oldMoveVector / 2f;
                speedCoef = slideSpeed;
                isSlide = false;
            }
        }

        //是否滑行到終點了 未完成
        if (Vector3.Distance(transform.position, movePoint.position) == 0f && isKnock)
        {
            isKnock = false;
        }

    }

    //其餘物件呼叫用撞擊 未完成
    public void knock(float impactDistance = 5, float impactSpeed = 10)
    {
        movePoint.position += new Vector3(impactDistance, 0f, 0f);
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, impactSpeed * Time.deltaTime);
        isSlide = false;
        isKnock = true;
    }
}
