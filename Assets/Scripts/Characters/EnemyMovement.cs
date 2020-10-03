using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private float bpm;
    public float Bpm { get { return bpm; } set { bpm = value; } }



    [SerializeField]
    private Vector2 pushPower;
    public Vector2 PushPower { get { return pushPower; } set { pushPower = value; } }


    public GameObject AttackDetecter;

    public float moveUnit;
    public Vector2 xBound;

    private Rigidbody2D rigid;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetFloat("IdleSpeedMultiplier", bpm / 60);
        StartCoroutine("Move");
   
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator Move()
    {
        bool direction = false;
        float moveFactor = bpm / 60 * moveUnit;
        while (true)
        {
            while (!direction)
            {  
                rigid.velocity = new Vector2(-moveFactor, 0);
                if (transform.position.x <= xBound.x)
                    direction = !direction;
                yield return null;
            }
            while (direction)
            {
                rigid.velocity = new Vector2(moveFactor, 0);
                if (transform.position.x >= xBound.y)
                    direction = !direction;
                yield return null;
            }


            yield return null;
        }
        //yield return new WaitForSeconds(bpm / 60.0f);
    }

    public void PushPlayer()
    {
        
    }
}
