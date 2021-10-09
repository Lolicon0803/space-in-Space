using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeWriterTest : MonoBehaviour
{
    public GameObject textWriter;
    public string fileName;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            textWriter.GetComponent<TextWriter>().Init();
            textWriter.GetComponent<TextWriter>().LoadStory(fileName + ".txt");
            textWriter.GetComponent<TextWriter>().NextStory();
        }


    }
}
