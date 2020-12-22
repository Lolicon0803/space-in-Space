using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class AccelerationZone : MonoBehaviour
{
    public float power;
    public int distance;

    private Vector2 direction;

    private bool hasWorked;

    private void Start()
    {
        hasWorked = false;
        direction = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * Vector2.right;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasWorked)
        {
            if (Vector2.Distance(Player.Singleton.movement.transform.position, transform.position) < Player.Singleton.movement.NowSpeed * Time.deltaTime)
            {
                Player.Singleton.movement.Knock(direction, distance, power, true);
                hasWorked = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            hasWorked = false;
        }
    }
}
