using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float leftX;
    public float rightX;
    public float upY;
    public float downY;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = new Vector3(Player.Singleton.transform.position.x, Player.Singleton.transform.position.y, -10);
        pos.x = Mathf.Clamp(pos.x, leftX, rightX);
        pos.y = Mathf.Clamp(pos.y, downY, upY);
        transform.position = pos;
    }
}
