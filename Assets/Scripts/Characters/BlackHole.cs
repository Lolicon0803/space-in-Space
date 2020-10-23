using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    // 節奏類型
    public TempoActionType tempoType;
    // 吸人時的速度
    public float impactSpeed;
    // 吸人時人的旋轉速度
    public float impactRotationSpeed;

    private Animator animator;
    private bool isActive;

    private readonly int activateTrigger = Animator.StringToHash("Activate");

    private void Awake()
    {
        isActive = false;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        ObjectTempoControl.Singleton.AddToBeatAction(Activate, tempoType);
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward, 90.0f * Time.deltaTime);
    }

    private void Activate()
    {
        //Debug.Log("Black Hole activate");
        animator.SetTrigger(activateTrigger);
        isActive = true;
    }

    private void Deactivate()
    {
        //Debug.Log("Black Hole dectivate");
        isActive = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive)
            return;

        if (collision.CompareTag("Player"))
        {
            //Debug.Log("Black Hole get player.");
            collision.GetComponent<PlayerMovement>().FallIntoBlackHole(this);
        }
    }

}
