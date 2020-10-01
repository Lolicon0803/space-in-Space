using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    private Rigidbody2D rigid;

    private bool isGotHit;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        rigid = GetComponent<Rigidbody2D>();
        isGotHit = false;
        StartCoroutine(Move());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator Move()
    {
        while (true)
        {
            if (!isGotHit)
            {
                // Move left.
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    rigid.velocity = new Vector2(-speed * Time.deltaTime, 0);
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                // Move right.
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    rigid.velocity = new Vector2(speed * Time.deltaTime, 0);
                    transform.localScale = new Vector3(1, 1, 1);
                }
                //Stop.
                else
                    rigid.velocity = Vector2.zero;
            }
            // Test
            if (Input.GetKeyDown(KeyCode.H))
                Damaged(new Vector2(20000, 14000));
            Debug.Log(rigid.velocity);
            yield return null;
        }
    }

    public void Damaged(Vector2 force)
    {
        isGotHit = true;
        force *= Time.deltaTime;
        rigid.AddForce(new Vector2(-transform.localScale.x * force.x, force.y));
        StartCoroutine(RecoverHit());
    }

    private IEnumerator RecoverHit()
    {
        yield return new WaitForSeconds(1.5f);
        isGotHit = false;
    }
}
