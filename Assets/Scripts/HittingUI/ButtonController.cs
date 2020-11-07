using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private SpriteRenderer renderer;

    public Color defaultColor = Color.white;
    public Color pressedColor = Color.grey;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        renderer.color = defaultColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            renderer.color = pressedColor;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            renderer.color = defaultColor;
        }
    }
}
