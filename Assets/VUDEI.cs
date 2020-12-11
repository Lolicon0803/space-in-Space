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
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //往右走
        if (Input.GetKeyDown(KeyCode.A))
        {
            gameObject.GetComponent<Animator>().SetBool("S", true);
            gameObject.transform.Rotate(new Vector3(0, 1, 0), -180);

        }

        //往左走
        if (Input.GetKeyDown(KeyCode.S))
        {
            gameObject.GetComponent<Animator>().SetBool("S", true);
            gameObject.transform.Rotate(new Vector3(0, 1, 0), 180);
        }
        
        //停下
        if (Input.GetKeyDown(KeyCode.D))
        {

            gameObject.GetComponent<Animator>().SetBool("S", false);
        }

        //進劇情
        if (Input.GetKeyDown(KeyCode.F))
        {

            textWriter.GetComponent<TextWriter>().Init();
            textWriter.GetComponent<TextWriter>().LoadStory("A1.txt");
            textWriter.GetComponent<TextWriter>().NextStory();
        }

        //睜開眼睛
        if (Input.GetKeyDown(KeyCode.G))
        {
          

        }

        //進劇情
        if (Input.GetKeyDown(KeyCode.H))
        {

            gameObject.GetComponent<Animator>().SetBool("S", true);
            gameObject.transform.Rotate(new Vector3(0, 1, 0), -180);
        }
    }
}
