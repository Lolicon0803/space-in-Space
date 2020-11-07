using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteObject : MonoBehaviour
{
    public bool canBePressed;

    private float movementSpeed;
    private NoteController noteController;

    // Start is called before the first frame update
    void Start()
    {
        noteController = GameObject.Find("NoteHolder").GetComponent<NoteController>();
        movementSpeed = noteController.movementSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (canBePressed)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                GameObject.Find("msg").GetComponent<Text>().text = "Great";
                Destroy(gameObject);
            }
        }

        if (transform.position.x > 0)
        {
            transform.Translate(-noteController.BPM * Time.deltaTime * movementSpeed, 0, 0);
            if (transform.position.x <= 0)
            {
                GameObject.Find("msg").GetComponent<Text>().text = "Miss";
                Destroy(gameObject);
            }
        }
        else if (transform.position.x < 0)
        {
            transform.Translate(noteController.BPM * Time.deltaTime * movementSpeed, 0, 0);
            if (transform.position.x >= 0)
            {
                GameObject.Find("msg").GetComponent<Text>().text = "Miss";
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HitButton"))
        {
            canBePressed = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("HitButton"))
        {
            canBePressed = false;
        }
    }
}
