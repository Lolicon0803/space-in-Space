using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Characters;






public class Meteor : MonoBehaviour, ObjectBehavier
{
    public Rigidbody2D rigid;
    public Vector2 first;
    // private float X = 0;
    // private float Y = 0;
    public float goal;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Move");
        first.x = rigid.position.x;
        first.y = rigid.position.y;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerMovement>().Knock(Vector2.left, 5,10);
        }

    }

    public IEnumerator Move()
    {
        float moveFactor = 1;

        while (true)
        {
            rigid.velocity = new Vector2(moveFactor, 0);
            if (rigid.position.x > goal)
            {
                moveFactor = 1;
                rigid.position = first;
    
            }
            moveFactor += (float)0.01;
            yield return null;
        }
    }

}
