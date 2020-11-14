using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SurfaceEffector2D), typeof(Ground))]
public class Conveyor : MonoBehaviour
{
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.collider.CompareTag("Player"))
    //    {
    //        Player.Singleton.movement.StopMove();
    //    }
    //}

    private void OnCollisionStay2D(Collision2D collision)
    {
        //if (collision.collider.CompareTag("Player"))
        //{
        //    Player.Singleton.transform.position += transform.right * speed * Time.deltaTime;
        //}
    }
}
