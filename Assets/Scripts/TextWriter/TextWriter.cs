using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Assets.Scripts.TextWriter;
enum TextWriterState {
    NextWord,
    NextLine,
    NextParagraph
}
public class TextWriter : MonoBehaviour
{
    SaveAndLoad SL;
    public AudioSource beepSound;
    public Text text;
    public Text speakerName;
    public SpriteRenderer speaker;
    public SpriteRenderer dialogBox;
    public Sprite[] speakerFrames;
    public UnityEvent onEndStory;
    private int wordIndex;
    private int paragraphIndex;
    private Story story;
    private bool enterKeyDown;
    private bool isEndOfParagraph;
    private bool isPrintingStory;
    private string currentText;
    private string twinkleText;
    private string currentFileName;

    public UnityAction textAction;



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
        paragraphIndex = 0;
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
                if (paragraphIndex + 1 >= story.Count) {isPrintingStory = false; }
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
                paragraphIndex++;
                ClearText();
                ClearSprite();
                if (!isPrintingStory)
                {
                    onEndStory.Invoke();
                    break;
                }
            }
        }


    }
    Sprite SpeakerNameToSpeakerFrame(string speakerName)
    {
        switch (speakerName)
        {
            case "夸克戴爾":
                if (DataBase.Singleton.datas.collectItems.ContainsKey("bag") &&
                    DataBase.Singleton.datas.collectItems["bag"])
                    return speakerFrames[1];
                else
                    return speakerFrames[9];
            case "安傑斯":
                return speakerFrames[2];
            case "鮑伯":
                return speakerFrames[3];
            case "莎夏":
                return speakerFrames[4];
            case "丹":
                return speakerFrames[5];
            case "艾克":
                return speakerFrames[6];
            case "奇爾星長老":
                return speakerFrames[7];
            case "奇爾星通訊兵":
                return speakerFrames[8];
            default:
                return speakerFrames[0];
        }
    }
    void ClearText()
    {
        currentText = "";
        foreach (var text in this.GetComponentsInChildren<Text>())
        {
            text.text = "";
        }
    }
    void ClearSprite()
    {
        speaker.enabled = false;
        dialogBox.enabled = false;
    }
    void ShowSprite()
    {
        speaker.enabled = true;
        speaker.sprite = SpeakerNameToSpeakerFrame(story[paragraphIndex].speaker);
        dialogBox.enabled = true;
    }
    IEnumerator PrintWord()
    {
        wordIndex = 0;
        isEndOfParagraph = false;
        while (true)
        {
            if (enterKeyDown)
            {
                enterKeyDown = false;
                wordIndex = story[paragraphIndex].text.Length;
                currentText = story[paragraphIndex].text;
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                currentText += story[paragraphIndex].text[wordIndex];
                wordIndex++;
                yield return new WaitForSeconds(0.1f);
            }
            beepSound.Play();
            if (wordIndex >= story[paragraphIndex].text.Length)
            {
                break;
            }

        }
        isEndOfParagraph = true;
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
        if (currentFileName == "" || paragraphIndex >= story.Count )
        {
        }
        else
        {
            // 展示當前講者及內容
            speakerName.text = story[paragraphIndex].speaker;
            text.text = currentText + twinkleText;
        }
        if (!isEndOfParagraph && Input.GetKeyDown(KeyCode.Return))
        {
            enterKeyDown = true;
        }

    }


    public void resolveLimit()
    {
        textAction();
    }

}
