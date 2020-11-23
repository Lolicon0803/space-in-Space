using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{
    public GameObject textWriter;
    public string storyName;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
  
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") && !DataBase.Singleton.number1Npc)
        {
            DataBase.Singleton.number1Npc = true;
            textWriter.GetComponent<TextWriter>().Init();
            textWriter.GetComponent<TextWriter>().LoadStory(storyName + ".txt");
            textWriter.GetComponent<TextWriter>().NextStory();
            Player.Singleton.movement.StopMove();
        }
    }


}
