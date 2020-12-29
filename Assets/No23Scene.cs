using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class No23Scene : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject textWriter;
    void Start()
    {
        StartCoroutine("PlayHint");
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator PlayHint()
    {
        yield return null;
        textWriter.GetComponent<TextWriter>().Init();
        textWriter.GetComponent<TextWriter>().LoadStory("23-0.txt");
        textWriter.GetComponent<TextWriter>().NextStory();
        Player.Singleton.movement.StopMove(false);
        Player.Singleton.movement.canInput = false;
    }
}
