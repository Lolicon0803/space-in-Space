using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermove3 : MonoBehaviour
{
    public float movespeed=5f;
    public float slidespeed = 1f;

    public Transform movePoint;
    public bool isSlide=false;
    public Vector3 oldMoveVector;

    public float dis = 2;

    private float speed = 1f;

    public bool isKnock = false;
    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent=null;
    }

    // Update is called once per frame
    void Update()
    {
      
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, movePoint.position) <= 0.2f)
            {
                speed = movespeed;
                //垂直
                if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
                {
                    if (Input.GetButton("Jump"))
                    {
                        //  Debug.Log("D");
                        dis = 2;
                        isSlide = true;
                    }
                    else
                    {
                        dis = 1;
                        isSlide = false;
                    }
                    oldMoveVector = new Vector3(dis * Input.GetAxisRaw("Horizontal"), 0f, 0f);
                    movePoint.position += oldMoveVector;
                    // isSlide = true;
                }
                else
                //水平 
                if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
                {
                    if (Input.GetButton("Jump"))
                    {
                        // Debug.Log("D");
                        dis = 2;
                        isSlide = true;
                    }
                    else
                    {
                        dis = 1;
                        isSlide = false;
                    }

                    oldMoveVector = new Vector3(0f, dis * Input.GetAxisRaw("Vertical"), 0f);
                    movePoint.position += oldMoveVector;

                }
                else
                //自然滑行一格
                if (isSlide)
                {
                    movePoint.position += oldMoveVector / 2;
                    speed = slidespeed;
                    isSlide = false;
                }
            }

            if (Vector3.Distance(transform.position, movePoint.position) == 0f)
            {
                isKnock = false;
            }
        
    }

    public void knock()
    {
     
        oldMoveVector = new Vector3(5f, 0f, 0f);
        movePoint.position += oldMoveVector;
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, 10 * Time.deltaTime);
        isSlide = false;
        isKnock = true;
        
    }
}
