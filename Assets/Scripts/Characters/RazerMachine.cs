using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Characters;



public class RazerMachine : MonoBehaviour
{
    public int razerDistance;
    public int razerDiracter;
    public bool[] razerTempo;
    public TempoActionType tempoType;

    public int waitTempo;
    public bool razerWaitStatus;
    private int razerTempoIndex = 0;

    //雷射音效 待全域化
    public AudioClip razerStart;
    public AudioClip razerPlaying;
    public AudioClip razerEnd;


    void Awake()
    {
        transform.position = new Vector2(Mathf.Floor(transform.position.x) + 0.5f, Mathf.Floor(transform.position.y) + 0.5f);
        transform.GetChild(0).transform.localRotation = Quaternion.Euler(0, 0, razerDiracter);
        transform.GetChild(0).transform.localScale = new Vector3(0, 1, 1);
    }

    // Start is called before the first frame update
    void Start()
    {
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
            Debug.Log("撞到雷射機 扣血");

            // Call損血系統(bool 扣多少血)
        }
    }

    public void Move()
    {
        if (waitTempo != 0)
        {
            waitTempo--;

            if (razerWaitStatus)
            {
                transform.GetChild(0).transform.localScale = new Vector3(razerDistance, 1, 1);

            }
            else
            {
                transform.GetChild(0).transform.localScale = new Vector3(0, 1, 1);
            }

        }
        else
        {
            if (razerTempo[razerTempoIndex])
            {
                transform.GetChild(0).transform.localScale = new Vector3(razerDistance, 1, 1);

                //持續播放音效
                if (!gameObject.GetComponent<AudioSource>().loop)
                {
                    gameObject.GetComponent<AudioSource>().clip = razerPlaying;
                    gameObject.GetComponent<AudioSource>().loop = true;
                    gameObject.GetComponent<AudioSource>().Play();
                }
            }
            else
            {
                transform.GetChild(0).transform.localScale = new Vector3(0, 1, 1);

                //關閉音效
                gameObject.GetComponent<AudioSource>().loop = false;
                gameObject.GetComponent<AudioSource>().Stop();
            }

            razerTempoIndex = (razerTempoIndex + 1) % razerTempo.Length;
        }

    }

    void CanMove()
    {
        Move();
    }

}
