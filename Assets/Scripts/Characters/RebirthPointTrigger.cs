using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RebirthPointTrigger : MonoBehaviour
{
    [Header("-1表示當前場景")]
    public int sceneIndex = -1;
    public Vector2 rebirthPoint;
    public bool triggered;

    private void Start()
    {
        if (rebirthPoint == new Vector2(0, 0))
        {
            rebirthPoint = transform.position;
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !triggered)
        {
            float d = Vector2.Distance(Player.Singleton.movement.transform.position, transform.position);
            if (d < Player.Singleton.movement.NowSpeed * Time.deltaTime
                || d < Mathf.Epsilon)
            {
                Debug.Log("重生點設置");
                Player.Singleton.lifeSystem.SetStartPosition(sceneIndex, rebirthPoint);
                DataBase.Singleton.Save();
                triggered = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        triggered = false;
    }
}
