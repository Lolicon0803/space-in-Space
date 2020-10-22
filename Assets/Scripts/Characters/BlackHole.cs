using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    // 是入口
    public bool isEntrance;
    //// 出口
    //public BlackHole exit;
    // 吸人時的速度
    public float impactSpeed;
    // 吸人範圍大小
    public float impactUnit;
    // 傳送時間
    public float sendTime;
    //// 是出口
    //public bool isExit;
    //// 推人範圍大小
    //public float pushUnit;
    //// 推人時的速度
    //public float pushSpeed;
    // 吸人時人的旋轉速度
    public float impactRotationSpeed;
    //// 推人時人的旋轉速度
    //public float pushRotationSpeed;
    //// 推人方向
    //public Vector2 pushDirection;

    public bool CanExit { get; private set; }

    private void Awake()
    {
        CanExit = false;
    }

    private void Update()
    {
        if (isEntrance)
        {
            LayerMask mask = LayerMask.GetMask("Player");
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, impactUnit * Vector2.one, 0, Vector2.zero, 0, mask);
            if (hit.collider != null)
                hit.collider.GetComponent<PlayerMovement>().FallIntoBlackHole(this);
        }
    }

    /// <summary>
    /// When sendTime is readched, player can exit black hole.
    /// </summary>
    /// <returns></returns>
    public IEnumerator WaitToExit()
    {
        CanExit = false;
        yield return new WaitForSeconds(sendTime);
        CanExit = true;
    }

    private void OnDrawGizmos()
    {
        if (isEntrance)
        {
            //if (exit == null)
            //    Debug.LogError("Teleporter " + gameObject.name + "has no exit!");
            //else
            //{
            //    Gizmos.color = Color.red;
            //    Gizmos.DrawLine(transform.position, exit.transform.position);
            //}
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, impactUnit * Vector2.one);
        }
        //if (isExit)
        //{
        //    Gizmos.color = Color.white;
        //    Gizmos.DrawLine(transform.position, transform.position + pushUnit * (Vector3)pushDirection);
        //}
    }
}
