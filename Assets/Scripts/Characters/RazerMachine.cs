using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Characters;



public class RazerMachine : MonoBehaviour
{
    public GameData.RazerData razerData;

    public bool[] razerTempo;
    public TempoActionType tempoType;

    public float razerSize;

    public int waitTempo;
    public bool razerWaitStatus;
    private int razerTempoIndex = 0;
    private RaycastHit2D hitObject;
    private bool isShooting = false;

    private Animator animator;
    private readonly int animationActiveKey = Animator.StringToHash("Active");

    //雷射音效 待全域化
    public AudioClip razerStart;
    public AudioClip razerPlaying;
    public AudioClip razerEnd;

    private Razer razer;

    void Awake()
    {
        animator = GetComponent<Animator>();
        razer = transform.GetChild(0).GetComponent<Razer>();
        razer.transform.Rotate(Vector3.forward, Mathf.Atan2(razerData.Direction.y, razerData.Direction.x) * Mathf.Rad2Deg);
    }

    // Start is called before the first frame update
    void Start()
    {
        ObjectTempoControl.Singleton.AddToBeatAction(CanMove, tempoType);
    }

    // Update is called once per frame
    void Update()
    {

        //if (isShooting)
        //{
        //    hitObject = Physics2D.BoxCast(transform.position, new Vector2(1, 1), 0, razerData.Direction, razerData.distance, LayerMask.GetMask("Player"));

        //    if (hitObject.collider != null)
        //    {
        //        Debug.Log("撞到雷射 即死");
        //        transform.GetChild(0).GetComponent<AudioSource>().Play();
        //       // Player.Singleton.lifeSystem.GameOver();
        //       Player.Singleton.lifeSystem.Hurt();
        //    }
        //}
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            Debug.Log("撞到雷射機 扣血");

            // Call損血系統
            Player.Singleton.lifeSystem.Hurt();
            Player.Singleton.movement.Knock(Vector2.zero);
        }
    }

    public void Move()
    {
        if (waitTempo != 0)
        {
            waitTempo--;

            if (razerWaitStatus)
            {
                razer.SetSize(razerData.distance, razerSize);
                animator.SetBool(animationActiveKey, true);
            }
            else
            {
                razer.SetSize(0, 0);
                animator.SetBool(animationActiveKey, false);
            }

        }
        else
        {
            if (razerTempo[razerTempoIndex])
            {
                razer.SetSize(razerData.distance, razerSize);
                animator.SetBool(animationActiveKey, true);
                isShooting = true;
                gameObject.GetComponent<AudioSource>().clip = razerPlaying;
                gameObject.GetComponent<AudioSource>().Play();
            }
            else
            {
                isShooting = false;
                razer.SetSize(0, 0);
                animator.SetBool(animationActiveKey, false);
                //關閉音效
                gameObject.GetComponent<AudioSource>().Stop();
            }

            razerTempoIndex = (razerTempoIndex + 1) % razerTempo.Length;
        }
    }

    void CanMove()
    {
        Move();
    }

    private void OnDrawGizmos()
    {
        for (int i = 1; i <= razerData.distance; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + (Vector3)razerData.Direction.normalized * i, Vector3.one);
        }

        Gizmos.DrawLine(transform.position, transform.position + (Vector3)razerData.Direction.normalized * razerData.distance);
    }

}
