using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class PlanetWalkingTrigger : MonoBehaviour
{
    public int direction = 0;
    public PlanetWalkingEffect planetWalkingEffect;
 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (direction != 0)
            {
                planetWalkingEffect.planetWalking(direction);
            }
        }
    }
}
