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
    SaveAndLoad SL;
    public GameObject[] textBoxs;
    private int wordIndex;
    private int paragraghIndex;
    private Story story;
    private bool keyDown;
    private bool isEndOfStory;
    // Start is called before the first frame update
    void Start()
    {
        SL = new SaveAndLoad();
        NextStory();
    }
    void LoadStory()
    {

        story = new Story();
        story.Add(new Paragraph(0, "A",
            "TESTTESTTESTTESTTESTTESTTESTTEST\n" +
            "TESTTESTTESTTESTTESTTESTTESTTEST"));
        story.Add(new Paragraph(1, "B",
            "TEST2TEST2TEST2TEST2TEST2TEST2\n" +
            "TEST2TEST2TEST2TEST2TEST2TEST2"
        ));
        //story = (Story)SL.LoadData(typeof(Story), "story1.txt[0]");
        SL.SaveData(story, "story1.txt");
    }
    void NextStory()
    {
        isEndOfStory = false;
        foreach (var item in textBoxs)
        {
            for (int childIndex = 0; childIndex < item.transform.childCount; childIndex++)
            {
                item.transform.GetChild(childIndex).GetComponent<Text>().text = "";
            }
        }
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
            
            if (paragraghIndex +1 >= story.Count) { isEndOfStory = true; }
            yield return StartCoroutine(WaitSignal(intervalTime, flickerTime));
            paragraghIndex++;
            foreach (var item in textBoxs)
            {
                for (int childIndex = 0; childIndex < item.transform.childCount; childIndex++)
                {
                    item.transform.GetChild(childIndex).GetComponent<Text>().text = "";
                }
            }
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
            textBoxs[story[paragraghIndex].textBoxIndex].transform.GetChild(0).GetComponent<Text>().text = story[paragraghIndex].speaker;
            textBoxs[story[paragraghIndex].textBoxIndex].transform.GetChild(1).GetComponent<Text>().text += story[paragraghIndex].text[wordIndex];
            wordIndex++;
            if (wordIndex >= story[paragraghIndex].text.Length)
            {
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    IEnumerator WaitSignal(float intervalTime, float flickerTime)
    {
        int textBoxIndex = story[paragraghIndex].textBoxIndex;
        while ((intervalTime >= 0 || isEndOfStory) && !keyDown )
        {
            intervalTime -= flickerTime;
            textBoxs[textBoxIndex].transform.GetChild(1).GetComponent<Text>().text += "_";
            yield return new WaitForSeconds(flickerTime);
            if (keyDown) { break; }
            intervalTime -= flickerTime;
            textBoxs[textBoxIndex].transform.GetChild(1).GetComponent<Text>().text =
                textBoxs[textBoxIndex].transform.GetChild(1).GetComponent<Text>().text
                .Substring(0, textBoxs[textBoxIndex].transform.GetChild(1).GetComponent<Text>().text.Length - 1);
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
