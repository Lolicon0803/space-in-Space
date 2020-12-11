using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class SceneChangeTrigger : MonoBehaviour
{
    [Header("切場景用")]
    public int targetIndex = -1;
    public Vector2 targetPosition;
    [Header("是否要與前一個場景相同x或相同y")]
    public bool sameX;
    public bool sameY;
    [Header("切畫面用")]
    [Tooltip("進入第二個畫面時相機位置")]
    public Vector3 cameraPositionIn;
    [Tooltip("回到第一個畫面時相機位置")]
    public Vector3 cameraPositionOut;
    [Header("切畫面後要觸發什麼事件")]
    [Tooltip("進入第二個畫面時的事件")]
    public UnityEvent eventInToHappen;
    [Tooltip("回到第一個畫面時的事件")]
    public UnityEvent eventOutToHappen;

    private bool inOut = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("S");
        if (collision.CompareTag("Player"))
        {
            Debug.Log("I");
            if (targetIndex != -1)
            {
                if (sameX)
                    targetPosition.x = Player.Singleton.transform.position.x;
                if (sameY)
                    targetPosition.y = Player.Singleton.transform.position.y;
                Player.Singleton.transform.position = targetPosition;
                ScenesManager.goToScene(targetIndex);
            }
            else
            {
                // 1 -> 2
                if (inOut)
                {
                    Camera.main.transform.position = cameraPositionOut;
                    inOut = !inOut;
                    eventOutToHappen.Invoke();
                }
                // 2 -> 1
                else
                {
                    Camera.main.transform.position = cameraPositionIn;
                    inOut = !inOut;
                    eventInToHappen.Invoke();
                }
            }
        }
    }
}
