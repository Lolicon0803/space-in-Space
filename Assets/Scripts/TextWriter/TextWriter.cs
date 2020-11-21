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
    public AudioSource beepSound;
    private int wordIndex;
    private int paragraghIndex;
    private Story story;
    private bool keyDown;
    private bool isEndOfStory;
    private bool isPrintingStory;
    private string currentText;
    private string twinkleText;
    private string currentFileName;
    // Start is called before the first frame update
    void Start()
    {
        SL = new SaveAndLoad();
        StartCoroutine(TwinkleString(0.3f));
        isPrintingStory = false;
        Init();
    }

    public void Init()
    {
        if (isPrintingStory) {  return ; }
        currentFileName = "";
        isEndOfStory = false;
        paragraghIndex = 0;
        wordIndex = 0;
        ClearText();
        ClearSprite();
    }
    public void LoadStory(string fileName)
    {
        Init();
        currentFileName = fileName;
        story = (Story)SL.LoadData(typeof(Story), fileName);
    }
    public void NextStory()
    {
        StartCoroutine(PrintStory());
    }
    IEnumerator PrintStory()
    {
        if (isPrintingStory) { yield return null; }
        else {
            isPrintingStory = true;
            while (true)
            {
                ShowSprite();
                yield return StartCoroutine(PrintWord());
                float intervalTime = 3.0f;
                float twinkleTime = 0.3f;
                if (paragraghIndex + 1 >= story.Count) { isEndOfStory = true; isPrintingStory = false; }
                yield return StartCoroutine(WaitSignal(intervalTime, twinkleTime));
                paragraghIndex++;
                ClearText();
                ClearSprite();
                if (!isPrintingStory)
                {
                    break;
                }
            }
        }
    }

    void ClearText()
    {
        currentText = "";
        foreach (var textBox in textBoxs)
        {
            foreach (var text in textBox.GetComponentsInChildren<Text>())
            {
                text.text = "";
            }
        }
    }
    void ClearSprite()
    {
        currentText = "";
        foreach (var textBox in textBoxs)
        {
            foreach (var sprite in textBox.GetComponentsInChildren<SpriteRenderer>())
            {
                sprite.enabled = false;
            }
        }
    }
    void ShowSprite()
    {
        foreach (var sprite in textBoxs[story[paragraghIndex].textBoxIndex].GetComponentsInChildren<SpriteRenderer>())
        {
            sprite.enabled = true;
        }
    }
    IEnumerator PrintWord()
    {
        wordIndex = 0;
        
        while (true)
        {
            currentText += story[paragraghIndex].text[wordIndex];
            wordIndex++;
            beepSound.Play();
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
            if (keyDown) { break; }
            intervalTime -= flickerTime;
            yield return new WaitForSeconds(flickerTime);
        }
    }
    IEnumerator TwinkleString(float flickerTime)
    {
        while (true)
        {
            twinkleText = "_";
            yield return new WaitForSeconds(flickerTime);
            twinkleText = "";
            yield return new WaitForSeconds(flickerTime);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (currentFileName == "" || paragraghIndex >= story.Count )
        {
        }
        else
        {
            textBoxs[story[paragraghIndex].textBoxIndex].transform.GetChild(0).GetComponent<Text>().text = story[paragraghIndex].speaker;
            textBoxs[story[paragraghIndex].textBoxIndex].transform.GetChild(1).GetComponent<Text>().text = currentText + twinkleText;
        }

        if (isEndOfStory && Input.anyKeyDown)
        {
            keyDown = true;
        }
    }
}
