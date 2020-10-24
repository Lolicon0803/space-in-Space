using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RebirthPointTrigger : MonoBehaviour
{
    public Vector2 rebirthPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player.Singleton.lifeSystem.SetStartPosition(rebirthPoint);
        }
    }
}
