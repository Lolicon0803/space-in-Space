using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlUV : MonoBehaviour
{
    private float speed = 0.0001f;
    private Material material;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        float uvX = material.GetVector("_Offset").x;
        float uvY = material.GetVector("_Offset").y;

        uvX += speed;
        uvY += speed/3;

        material.SetVector("_Offset", new Vector4(uvX, uvY, 0.0f, 0.0f));
    }
}
