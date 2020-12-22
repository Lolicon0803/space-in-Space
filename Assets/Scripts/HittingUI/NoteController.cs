using System.Collections;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    public HittingController hittingController;
    public bool hasStarted;
    private float lastDelay;

    private float notesGenerationInterval;
    private float initNotesGenerationInterval;
    private bool isRunning;
    private GameObject notePrefab;
    private float SPB;
    private double delay;
    private int counter;
    const float noteInitPositionX = 7f - 0.1f;

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

    public void setLastDelay(float value)
    {
        lastDelay = value / 1000;
    }

    IEnumerator initVariables()
    {
        // 有可能執行時 audioEngine 尚未設定完成 所以等到完成才設定參數
        yield return new WaitUntil(() => hittingController.audioEngine.BPM > 0);

        notePrefab = hittingController.notePrefab;
        isRunning = false;
        hasStarted = true;
        lastDelay = 0;
        counter = 0;
        SPB = (float)(1 / ((this.hittingController.audioEngine.BPM) / 60));
        delay = hittingController.audioEngine.GetCurrentDelayTime() / 1000;
        notesGenerationInterval = (float)(60.0d / (hittingController.audioEngine.BPM));
        initNotesGenerationInterval = notesGenerationInterval;
    }
    IEnumerator GenerateNotes()
    {
        isRunning = true;

        fillBlankWithNote();

        while (hasStarted)
        {
            GameObject leftNote = Instantiate(notePrefab, transform);
            GameObject rightNote = Instantiate(notePrefab, transform);

            rightNote.transform.Translate(noteInitPositionX, 0, 0);
            leftNote.transform.Translate(-noteInitPositionX, 0, 0);

            correctGenerateSpeed();

            yield return new WaitUntil(() => !hittingController.isStop());
            yield return new WaitForSeconds(notesGenerationInterval);
        }

        isRunning = false;
    }

    void correctGenerateSpeed()
    {
        if (Mathf.Abs((float)(lastDelay - delay)) > 0.025)
        {
            counter++;
            if (counter > 5)
            {
                if (lastDelay >= 0 && lastDelay <= SPB / 2)
                {
                    notesGenerationInterval += (float)(delay - lastDelay) / 100;
                }
                else
                {
                    notesGenerationInterval += (float)((SPB - lastDelay + delay) / 100);
                }
                counter = 0;
            }
        }
        else
        {
            counter = 0;
            notesGenerationInterval = initNotesGenerationInterval;
        }
    }

    void fillBlankWithNote()
    {
        for (int i = 5; i >= 0; i--)
        {
            GameObject leftNote = Instantiate(notePrefab, transform);
            GameObject rightNote = Instantiate(notePrefab, transform);

            rightNote.transform.Translate(noteInitPositionX - 1.015f * (i + 1), 0, 0);
            leftNote.transform.Translate(-noteInitPositionX + 1.015f * (i + 1), 0, 0);
        }
    }
}
