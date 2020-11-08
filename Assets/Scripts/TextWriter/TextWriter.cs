using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.TextWriter;
enum TextWriterState {
    NextWord,
    NextLine,
    NextParagraph
}
public class TextWriter : MonoBehaviour
{

    public Text txt;
    private int wordIndex;
    private int paragraghIndex;
    private Story story;
    private bool keyDown;
    private bool isEndOfStory;
    // Start is called before the first frame update
    void Start()
    {
        NextStory();
    }
    void LoadStory()
    {
        story = new Story();
        story.Add(
            "TESTTESTTESTTESTTESTTESTTESTTEST\n" +
            "TESTTESTTESTTESTTESTTESTTESTTEST"
            );
        story.Add(
            "TEST2TEST2TEST2TEST2TEST2TEST2\n" +
            "TEST2TEST2TEST2TEST2TEST2TEST2"
            );
    }
    void NextStory()
    {
        isEndOfStory = false;
        txt.text = "";
        LoadStory();
        StartCoroutine(PrintStory());
    }
    IEnumerator PrintStory()
    {
        keyDown = false;
        paragraghIndex = 0;
        while (true)
        {
            yield return StartCoroutine(PrintWord());
            float intervalTime = 3.0f;
            float flickerTime = 0.3f;
            paragraghIndex++;
            if (paragraghIndex >= story.Count) { isEndOfStory = true; }
            yield return StartCoroutine(WaitSignal(intervalTime, flickerTime));
            txt.text = "";
            if (isEndOfStory)
            {
                break;
            }
        }

    }
    IEnumerator PrintWord()
    {
        wordIndex = 0;
        while (true)
        {
            txt.text += story[paragraghIndex][wordIndex];
            wordIndex++;
            if (wordIndex >= story[paragraghIndex].Length)
            {
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    IEnumerator WaitSignal(float intervalTime, float flickerTime)
    {
        while ((intervalTime >= 0 || isEndOfStory) && !keyDown )
        {
            intervalTime -= flickerTime;
            txt.text += "_";
            yield return new WaitForSeconds(flickerTime);
            if (keyDown) { break; }
            intervalTime -= flickerTime;
            txt.text = txt.text.Substring(0, txt.text.Length - 1);
            yield return new WaitForSeconds(flickerTime);
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (isEndOfStory && Input.anyKeyDown)
        {
            keyDown = true;
            Invoke("NextStory",0.5f);
        }
    }
}
