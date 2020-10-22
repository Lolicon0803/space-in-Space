using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    // 是入口
    public bool isEntrance;
    // 出口
    public Teleporter exit;
    // 傳送時間
    public float sendTime;
    // 是出口
    public bool isExit;
    // 推人推多遠
    public float pushUnit;
    // 推人時的速度
    public float pushSpeed;
    // 吸人時人的旋轉速度
    public float impactRotationSpeed;
    // 推人時人的旋轉速度
    public float pushRotationSpeed;
    // 推人方向
    public Vector2 pushDirection;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            if (Vector2.Distance(transform.position, player.movePoint.position) <= Constants.moveUnit)
                player.Teleport(this, exit);
        }
    }

    /// <summary>
    /// When sendTime is readched, player can exit black hole.
    /// </summary>
    /// <returns></returns>
    public IEnumerator WaitToExit()
    {
        yield return new WaitForSeconds(sendTime);
    }

    private void OnDrawGizmos()
    {
        if (isEntrance)
        {
            if (exit == null)
                Debug.LogError("Telepoter " + gameObject.name + " has no exit.");
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, exit.transform.position);
        }
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + pushUnit * (Vector3)pushDirection);
    }
}
