using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAspect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UpdateAspect();
    }

    public void UpdateAspect()
    {
        // 開發時設定大小 * 期望解析度 / 實際解析度
        Camera.main.orthographicSize = 5.0f * (16.0f / 9.0f) / ((float)Screen.width / Screen.height);
    }
}
