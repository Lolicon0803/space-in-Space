using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Npc : MonoBehaviour
{

    public GameObject textWriter;

    //要播放的故事
    public string[] storyName;

    //前置故事名
    public string[] postConditionStoryName;

    // 是否需要重複對話
    public bool[] needReapeat;

    // 重複對話
    public string[] reapeatStory;

    //執行完對話是否需要銷毀?(用途:物品)
    public bool isdestroy;
    // 暫，對話完可以拿到的物品
    public string item;
    private int count = 0;

    private bool isSpace = false;
    public void canSpace()
    {
        isSpace = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (item == null)
            item = "";
        // 是道具，玩家已取得，載入後刪掉
        if (DataBase.Singleton.datas.collectItems.ContainsKey(item) && DataBase.Singleton.datas.collectItems[item])
        {
            if (isdestroy)
                Destroy(gameObject);
        }

        if (storyName[0] != "SpaceBoat")
        {


            textWriter.GetComponent<TextWriter>().textAction += canSpace;

            DataBase.Singleton.datas.readStories["false"] = false;
            DataBase.Singleton.datas.readStories["true"] = true;

            // 資料庫不包含劇情，把所有劇情新增到資料庫
            if (!DataBase.Singleton.datas.readStories.ContainsKey(storyName[0]))
            {
                foreach (string element in storyName)
                {
                   
                    DataBase.Singleton.datas.readStories[element] = false;
                }

            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (storyName[0] == "SpaceBoat")
        {
            float dist = Vector3.Distance(Player.Singleton.transform.position, transform.position);
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (dist < 1.5f)
                {
                    gameObject.GetComponent<AudioSource>().Play();
                    StartCoroutine("Move1");
                }
            }
        }





        else if (count < storyName.Length && storyName[0] != "SpaceBoat" && storyName[0] != "SpaceBoats")
        {

            if (!DataBase.Singleton.datas.readStories[storyName[count]] && DataBase.Singleton.datas.readStories[postConditionStoryName[count]])
            {
                gameObject.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }



            //提示任務
            float dist = Vector3.Distance(Player.Singleton.transform.position, transform.position);


            if (dist < 1.5f)
            {
                gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;

                if (Input.GetKeyDown(KeyCode.F))
                {
                    Debug.Log(isSpace);
                    if (!isSpace)
                    {
                        PlayStory();
                    }
                }
            }
            else
            {
                gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
            }

            if (DataBase.Singleton.datas.readStories[storyName[count]] && DataBase.Singleton.datas.readStories[postConditionStoryName[count]])
            {
                count++;
            }
        }

        //讀過劇情或前置條件未完成則移除任務標示

        if (count >= storyName.Length)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }


    }

    public void ActivatePlayerInput()
    {
        Player.Singleton.movement.canInput = true;
    }

    public void PlayStory()
    {


        //如果完成了前置且沒讀過劇情，放劇情
        if (!DataBase.Singleton.datas.readStories[storyName[count]] && DataBase.Singleton.datas.readStories[postConditionStoryName[count]])
        {
            textWriter.GetComponent<TextWriter>().Init();
            textWriter.GetComponent<TextWriter>().LoadStory(storyName[count] + ".txt");
            textWriter.GetComponent<TextWriter>().NextStory();
            Player.Singleton.movement.StopMove(false);
            Player.Singleton.movement.canInput = false;
            isSpace = true;
            textWriter.GetComponent<TextWriter>().onEndStory.AddListener(OnEndStory);
        }
        //如果沒達成前置
        else if (!DataBase.Singleton.datas.readStories[postConditionStoryName[count]])
        {
            //如果需要重複對話
            if (needReapeat[count])
            {
                textWriter.GetComponent<TextWriter>().Init();
                textWriter.GetComponent<TextWriter>().LoadStory(reapeatStory[count] + ".txt");
                textWriter.GetComponent<TextWriter>().NextStory();
                Player.Singleton.movement.StopMove();
                Player.Singleton.movement.canInput = false;
                isSpace = true;
            }
        }
    }


    public void OnEndStory()
    {
        DataBase.Singleton.datas.readStories[storyName[count]] = true;
        if (item != null)
        {
            if (!DataBase.Singleton.datas.collectItems.ContainsKey(item) || !DataBase.Singleton.datas.collectItems[item])
            {
                DataBase.Singleton.datas.collectItems[item] = true;
                if (isdestroy)
                {
                    textWriter.GetComponent<TextWriter>().onEndStory.RemoveListener(OnEndStory);
                    Destroy(gameObject);
                }
            }
        }
        textWriter.GetComponent<TextWriter>().onEndStory.RemoveListener(OnEndStory);
    }

    public GameObject obj1 = null;
    private IEnumerator Move1()
    {
        while (Vector2.Distance(transform.position, obj1.transform.position) > 0)
        {
            Player.Singleton.transform.position = (Vector3)Vector2.MoveTowards(transform.position, obj1.transform.position, 1.5f * Time.deltaTime);
            // 移動
            transform.position = (Vector3)Vector2.MoveTowards(transform.position, obj1.transform.position, 1.5f * Time.deltaTime);
            yield return null;
        }
    }

}
