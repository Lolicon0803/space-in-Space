using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundmoveScript : MonoBehaviour
{
    private float speed = 0.0001f;

    public bool isHorital = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float new_x = GetComponent<Renderer>().material.GetTextureOffset("_MainTex").x;
        float new_y = GetComponent<Renderer>().material.GetTextureOffset("_MainTex").y;
        

        if (isHorital)
        {
            GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(new_x + speed, 0));
        }
        else
        {
            GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(new_x + speed, new_y + speed / 3));
        }
    }
}
