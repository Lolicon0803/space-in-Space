using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Characters;

public class WhiteHole : MonoBehaviour
{
    // 範圍半徑
    public float radius;
    public TempoActionType actionType;
    // 推人推多遠
    public int pushUnit;
    // 推人時的速度
    public float pushSpeed;

    public AudioClip pushPlayerAudio;

    private Animator animator;
    private bool isActive;

    private AudioSource audioSource;

    private LayerMask layerMask;

    private Vector2[] fourDirections;

    private readonly int activateTrigger = Animator.StringToHash("Activate");
    private readonly int animSpeed = Animator.StringToHash("Speed");

    private void Awake()
    {
        isActive = false;
        layerMask = LayerMask.GetMask("Player");
        fourDirections = new Vector2[] { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        ObjectTempoControl.Singleton.AddToBeatAction(Activate, actionType);
        if (TempoManager.Singleton.beatPerMinute > 60.0f)
            animator.SetFloat(animSpeed, TempoManager.Singleton.beatPerMinute / 60.0f);
    }

    private void Update()
    {
        //transform.Rotate(Vector3.forward, 90.0f * Time.deltaTime);
        if (isActive)
        {
            if (Physics2D.OverlapCircle(transform.position, radius, layerMask))
            {
                Vector2 pos = Player.Singleton.transform.position - transform.position;
                int pushIndex = 0;
                float minD = Vector2.Distance(pos, fourDirections[0]);
                for (int i = 1; i < fourDirections.Length; i++)
                {
                    float d = Vector2.Distance(pos, fourDirections[i]);
                    if (d < minD)
                    {
                        minD = d;
                        pushIndex = i;
                    }
                }
                Player.Singleton.movement.Knock(fourDirections[pushIndex], pushUnit, pushSpeed, false, true);
                audioSource.PlayOneShot(pushPlayerAudio);
                isActive = false;
            }
        }
    }

    private void Activate()
    {
        //Debug.Log("White Hole activate");
        animator.SetTrigger(activateTrigger);
        isActive = true;
    }

    private void Deactivate()
    {
        //Debug.Log("White Hole dectivate");
        isActive = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
