using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationZone : MonoBehaviour
{
    public float scaleCoefficient;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            Player.Singleton.movement.SpeedUp(scaleCoefficient);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            Player.Singleton.movement.SpeedDown(scaleCoefficient);
    }
}
