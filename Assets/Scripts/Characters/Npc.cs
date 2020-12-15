﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private int count = 0;

    private bool isSpace = false;
    public void canSpace()
    {
        isSpace = false;
    }

    // Start is called before the first frame update
    void Start()
    {

        if (storyName[0] != "SpaceBoat")
        {


            textWriter.GetComponent<TextWriter>().textAction += canSpace;

            DataBase.Singleton.readStories["false"] = false;
            DataBase.Singleton.readStories["true"] = true;

            // 資料庫不包含劇情，把所有劇情新增到資料庫
            if (!DataBase.Singleton.readStories.ContainsKey(storyName[0]))
            {
                foreach (string element in storyName)
                {
                   
                    DataBase.Singleton.readStories[element] = false;
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
                    StartCoroutine("Move1");
                }
            }
        }





        else if (count < storyName.Length && storyName[0] != "SpaceBoat" && storyName[0] != "SpaceBoats")
        {

            if (!DataBase.Singleton.readStories[storyName[count]] && DataBase.Singleton.readStories[postConditionStoryName[count]])
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

            if (DataBase.Singleton.readStories[storyName[count]] && DataBase.Singleton.readStories[postConditionStoryName[count]])
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
        if (!DataBase.Singleton.readStories[storyName[count]] && DataBase.Singleton.readStories[postConditionStoryName[count]])
        {
            DataBase.Singleton.readStories[storyName[count]] = true;
            textWriter.GetComponent<TextWriter>().Init();
            textWriter.GetComponent<TextWriter>().LoadStory(storyName[count] + ".txt");
            textWriter.GetComponent<TextWriter>().NextStory();
            Player.Singleton.movement.StopMove(false);
            Player.Singleton.movement.canInput = false;
            isSpace = true;
        }
        //如果沒達成前置
        else if (!DataBase.Singleton.readStories[postConditionStoryName[count]])
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


    public GameObject obj1 = null;
    private IEnumerator Move1()
    {
        while (Vector2.Distance(transform.position, obj1.transform.position) > 0)
        {
            Player.Singleton.transform.position = (Vector3)Vector2.MoveTowards(transform.position, obj1.transform.position, 0.015f);
            // 移動
            transform.position = (Vector3)Vector2.MoveTowards(transform.position, obj1.transform.position, 0.015f);
            yield return null;
        }
    }

}
