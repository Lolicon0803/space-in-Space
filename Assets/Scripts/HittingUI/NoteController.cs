using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    public float BPM;
    public bool hasStarted;
    public GameObject notePrefab;
    public float noteInitPositionX = 6.0f;
    public float movementSpeed = 1.4f;

    // Start is called before the first frame update
    void Start()
    {
        BPM /= 60f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasStarted)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                hasStarted = true;
                StartCoroutine(GenerateNotes());
                Time.timeScale = 1;
            }
        } 
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                hasStarted = false;
                Time.timeScale = 0;
            }
        }
    }

    IEnumerator GenerateNotes()
    {
        while (hasStarted)
        {
            GameObject leftNote = Instantiate(notePrefab, transform);
            GameObject rightNote = Instantiate(notePrefab, transform);

            rightNote.transform.Translate(noteInitPositionX, 0, 0);
            leftNote.transform.Translate(-noteInitPositionX, 0, 0);

            yield return new WaitForSeconds(1.0f / BPM);
        }
    }
}
