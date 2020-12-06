using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundmoveScript : MonoBehaviour
{
    private float speed = 0.001f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float new_x = GetComponent<Renderer>().material.GetTextureOffset("_MainTex").x;
        GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(new_x + speed, 0));
    }
}
