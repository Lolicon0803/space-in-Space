using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextObject : MonoBehaviour
{
    public float endTime = 2f;
    private double counter;
    // Start is called before the first frame update
    void Start()
    {
        counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;

        transform.Translate(0, 0.002f, 0);

        Color oldColor = GetComponent<Text>().color;
        GetComponent<Text>().color = new Color(oldColor.r, oldColor.g, oldColor.b, oldColor.a - 0.003f);
        if (counter >= endTime)
        {
            Destroy(gameObject);
        }
    }
}
