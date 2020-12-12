using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{
    public GameObject textWriter;
    public string storyName;
    public bool isdestroy;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
     
    }

    public void ActivatePlayerInput()
    {
        Player.Singleton.movement.canInput = true;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") && !DataBase.Singleton.readStories.ContainsKey(storyName))
        {
            DataBase.Singleton.readStories[storyName] = true;
            textWriter.GetComponent<TextWriter>().Init();
            textWriter.GetComponent<TextWriter>().LoadStory(storyName + ".txt");
            textWriter.GetComponent<TextWriter>().NextStory();
            Player.Singleton.movement.StopMove(false);
            Player.Singleton.movement.canInput = false;

            if (isdestroy)
            {
                DataBase.Singleton.collectItems[storyName] = true;
                Destroy(gameObject);
            }
        }
    }


}
