using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class PlanetWalkingTrigger : MonoBehaviour
{
    [Header("切場景用")]
    public int targetIndex = -1;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (targetIndex != -1)
            {
            }
        }
    }
}
