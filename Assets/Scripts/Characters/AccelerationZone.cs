using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class AccelerationZone : MonoBehaviour
{
    public float scaleCoefficient;

    private Vector2 direction;

    private void Start()
    {
        direction = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * Vector2.right;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            Player.Singleton.movement.SpeedUp(scaleCoefficient);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player.Singleton.movement.Knock(direction, 1, Player.Singleton.movement.NowSpeed / scaleCoefficient, true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            Player.Singleton.movement.SpeedDown(scaleCoefficient);
    }
}
