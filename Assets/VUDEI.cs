using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class VUDEI : MonoBehaviour
{
    public GameObject textWriter;

    /// <summary>
    /// 模拟按键  按键对应表：http://www.doc88.com/p-895906443391.html
    /// </summary>
    /// <param name="bvk">虚拟键值 ESC键对应的是27</param>
    /// <param name="bScan">0</param>
    /// <param name="dwFlags">0为按下，1按住，2释放</param>
    /// <param name="dwExtraInfo">0</param>
    [DllImport("user32.dll", EntryPoint = "keybd_event")]
    public static extern void Keybd_event(byte bvk, byte bScan, int dwFlags, int dwExtraInfo);

    //  Keybd_event(65, 0, 1, 0);

    // Start is called before the first frame update
    public GameObject obj1;
    public GameObject obj2;
    public GameObject obj3;
    float clock = 0f;

    bool stop = false;

    bool[] eventTrigger = new bool[] { false, false, false };

    int count = 0;

    float voiceSpeed = 0.0001f;

    void Start()
    {

    }

    public void GoContinue()
    {

        stop = false;
    }


    // Update is called once per frame
    void Update()
    {
        Debug.Log(clock);

        if (voiceSpeed > 0 && this.GetComponent<AudioSource>().volume > 0.2f)
        {
            voiceSpeed = -voiceSpeed;
        }

        if (voiceSpeed < 0 && this.GetComponent<AudioSource>().volume < 0.08f)
        {
            voiceSpeed = -voiceSpeed;
        }


        this.GetComponent<AudioSource>().volume += voiceSpeed;

 


        if (!stop)
        {
            clock += Time.deltaTime;
        }

        // ....
        if (clock > 3 && count == 0)
        {
            textWriter.GetComponent<TextWriter>().Init();
            textWriter.GetComponent<TextWriter>().LoadStory("Anime1.txt");
            textWriter.GetComponent<TextWriter>().NextStory();
            stop = true;
            count++;
        }

        // 啊...
        if (clock > 6 && count == 1)
        {
            textWriter.GetComponent<TextWriter>().Init();
            textWriter.GetComponent<TextWriter>().LoadStory("Anime2.txt");
            textWriter.GetComponent<TextWriter>().NextStory();
            stop = true;
            count++;
        }

        //睜眼
        if (clock > 9 && count == 2)
        {
            Keybd_event(121, 0, 1, 0);
            count++;
        }

        // ... 好痛
        if (clock > 18 && count == 3)
        {
            textWriter.GetComponent<TextWriter>().Init();
            textWriter.GetComponent<TextWriter>().LoadStory("Anime3.txt");
            textWriter.GetComponent<TextWriter>().NextStory();
            stop = true;
            count++;
        }

        //左轉
        if (clock > 20 && count == 4)
        {
            transform.localScale = new Vector3(2, 2, 1);
            count++;
        }


        // ......
        if (clock > 21 && count == 5)
        {
            textWriter.GetComponent<TextWriter>().Init();
            textWriter.GetComponent<TextWriter>().LoadStory("Anime4.txt");
            textWriter.GetComponent<TextWriter>().NextStory();
            stop = true;
            count++;
        }

        //右轉
        if (clock > 23 && count == 6)
        {
            transform.localScale = new Vector3(-2, 2, 1);
            count++;
        }

        //......
        if (clock > 24 && count == 7)
        {
            textWriter.GetComponent<TextWriter>().Init();
            textWriter.GetComponent<TextWriter>().LoadStory("Anime4.txt");
            textWriter.GetComponent<TextWriter>().NextStory();
            stop = true;
            count++;
        }

        //移動到A
        if (clock > 27 && count == 8)
        {
            gameObject.GetComponent<Animator>().SetBool("S", true);

            StartCoroutine("Move1");
            count++;
        }

        // 我怎麼會在這裡
        if (clock > 32 && count == 9)
        {
            gameObject.GetComponent<Animator>().SetBool("S", false);

            textWriter.GetComponent<TextWriter>().Init();
            textWriter.GetComponent<TextWriter>().LoadStory("Anime5.txt");
            textWriter.GetComponent<TextWriter>().NextStory();
            stop = true;
            count++;
        }

        //移動到B
        if (clock > 35 && count == 10)
        {
            transform.localScale = new Vector3(2, 2, 1);
            gameObject.GetComponent<Animator>().SetBool("S", true);
            StartCoroutine("Move2");

            count++;
        }

        // 發生什麼事？什麼都想不起來。
        if (clock > 39 && count == 11)
        {
            gameObject.GetComponent<Animator>().SetBool("S", false);

            textWriter.GetComponent<TextWriter>().Init();
            textWriter.GetComponent<TextWriter>().LoadStory("Anime6.txt");
            textWriter.GetComponent<TextWriter>().NextStory();
            stop = true;
            count++;
        }

        // 總之先到處看看吧
        if (clock > 42 && count == 12)
        {
            textWriter.GetComponent<TextWriter>().Init();
            textWriter.GetComponent<TextWriter>().LoadStory("Anime7.txt");
            textWriter.GetComponent<TextWriter>().NextStory();
            stop = true;
            count++;
        }

        // 出去
        if (clock > 44 && count == 13)
        {
            transform.localScale = new Vector3(-2, 2, 1);
            gameObject.GetComponent<Animator>().SetBool("S", true);
            StartCoroutine("Move3");
            count++;
        }

    }

    private IEnumerator Move1()
    {
        while (Vector2.Distance(transform.position, obj1.transform.position) > 0)
        {
            // 移動
            transform.position = (Vector3)Vector2.MoveTowards(transform.position, obj1.transform.position, 0.015f);
            yield return null;
        }
    }

    private IEnumerator Move2()
    {
        while (Vector2.Distance(transform.position, obj2.transform.position) > 0)
        {
            // 移動
            transform.position = (Vector3)Vector2.MoveTowards(transform.position, obj2.transform.position, 0.01f);
            yield return null;
        }
    }

    private IEnumerator Move3()
    {
        while (Vector2.Distance(transform.position, obj3.transform.position) > 0)
        {
            // 移動
            transform.position = (Vector3)Vector2.MoveTowards(transform.position, obj3.transform.position, 0.02f);
            yield return null;
        }
    }
}
