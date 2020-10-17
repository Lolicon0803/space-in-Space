using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Characters;



public class RazerMachine : MonoBehaviour, IObjectBehavier
{
    public Rigidbody2D rigid;

    public int razerDistance;
    public int razerDiracter;
    public bool[] razerTempo;


    void Awake()
    {
        transform.position = new Vector2(Mathf.Floor(transform.position.x) + 0.5f, Mathf.Floor(transform.position.y) + 0.5f);
        transform.GetChild(0).transform.localRotation = Quaternion.Euler(0, 0, razerDiracter);
        transform.GetChild(0).transform.localScale = new Vector3(0, 1, 1);
    }

    // Start is called before the first frame update
    void Start()
    {
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
            Debug.Log("撞到雷射機 扣血");

            // Call損血系統(bool 扣多少血)
        }
    }

    public IEnumerator Move()
    {
        while (true)
        {
            foreach (bool item in razerTempo)
            {
                if (item)
                {
                    transform.GetChild(0).transform.localScale = new Vector3(razerDistance, 1, 1);
                    
                }
                else
                {
                    transform.GetChild(0).transform.localScale = new Vector3(0, 1, 1);
                }

                // Todo:接節奏API
                yield return new WaitForSeconds(1);
            }
        }

    }


}
