using System.Collections;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    public TempoManager tempoManager;
    public bool hasStarted;
    public GameObject notePrefab;
    public float noteInitPositionX = 6.0f;
    public float movementSpeed = 1.4f;
    public float generationSpeed = 1.4f;
    private float notesGenerationInterval;
    private bool isRunning;

    // Start is called before the first frame update
    void Start()
    {
        isRunning = false;
        hasStarted = true;
        notesGenerationInterval = (float)(60.0d / (tempoManager.BPM * generationSpeed));
    }

    // Update is called once per frame
    void Update()
    {
        if (hasStarted && !isRunning)
        {
            StartCoroutine(GenerateNotes());
        }
    }

    IEnumerator GenerateNotes()
    {
        isRunning = true;
        while (hasStarted)
        {
            GameObject leftNote = Instantiate(notePrefab, transform);
            GameObject rightNote = Instantiate(notePrefab, transform);

            rightNote.transform.Translate(noteInitPositionX, 0, 0);
            leftNote.transform.Translate(-noteInitPositionX, 0, 0);

            yield return new WaitForSeconds(notesGenerationInterval);
        }
        isRunning = false;
    }
}
