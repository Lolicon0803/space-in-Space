using System.Collections;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    public HittingController hittingController;
    public bool hasStarted;

    private float notesGenerationInterval;
    private bool isRunning;
    private GameObject notePrefab;
    private float noteInitPositionX = 3f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(initVariables());
    }

    // Update is called once per frame
    void Update()
    {
        if (hasStarted && !isRunning)
        {
            StartCoroutine(GenerateNotes());
        }
    }

    IEnumerator initVariables()
    {
        // 有可能執行時 audioEngine 尚未設定完成 所以等到完成才設定參數
        yield return new WaitUntil(() => hittingController.audioEngine.BPM > 0);

        notePrefab = hittingController.notePrefab;
        isRunning = false;
        hasStarted = true;
        notesGenerationInterval = (float)(60.0d / (hittingController.audioEngine.BPM));
    }
    IEnumerator GenerateNotes()
    {
        isRunning = true;

        // fillBlankWithNote();

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

    void fillBlankWithNote()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject leftNote = Instantiate(notePrefab, transform);
            GameObject rightNote = Instantiate(notePrefab, transform);

            rightNote.transform.Translate(noteInitPositionX - 1.1f * (i + 1), 0, 0);
            leftNote.transform.Translate(-noteInitPositionX + 1.1f * (i + 1), 0, 0);
        }
    }
}
