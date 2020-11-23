using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RebirthPointTrigger : MonoBehaviour
{
    [Header("-1表示當前場景")]
    public int sceneIndex = -1;
    public Vector2 rebirthPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player.Singleton.lifeSystem.SetStartPosition(sceneIndex, rebirthPoint);
        }
    }
}
