using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using UnityEngine.SceneManagement;

[Serializable]
public class Datas
{
    public Dictionary<string, bool> readStories;

    public Dictionary<string, bool> collectItems;

    public PlayerData playerData;
}

public class DataBase : MonoBehaviour
{
    public Datas datas;

    private SaveAndLoad sl;

    private static DataBase singleton;

    public static DataBase Singleton
    {
        get
        {
            if (singleton == null)
            {
                singleton = FindObjectOfType(typeof(DataBase)) as DataBase;
            }
            return singleton;
        }

    }

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            datas = new Datas
            {
                readStories = new Dictionary<string, bool>(),
                collectItems = new Dictionary<string, bool>(),
                playerData = new PlayerData()
            };
            sl = new SaveAndLoad();
            DontDestroyOnLoad(gameObject);
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Save();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }
    }

    public void Save()
    {
        SavePlayerData();
        sl.SaveData(datas, "PlaySave/save.json", true);
    }

    private void SavePlayerData()
    {
        if (Player.Singleton != null)
        {
            datas.playerData.position = Player.Singleton.transform.position;
            datas.playerData.rotation = Player.Singleton.transform.rotation.eulerAngles;
            datas.playerData.scale = Player.Singleton.transform.localScale;
            if (Player.Singleton.lifeSystem != null)
            {
                datas.playerData.nowHp = Player.Singleton.lifeSystem.NowHp;
                datas.playerData.rebirthSceneIndex = SceneManager.GetActiveScene().buildIndex;
                datas.playerData.rebirthPosition = Player.Singleton.transform.position;
                datas.playerData.nowHp = Player.Singleton.lifeSystem.NowHp;
            }
        }
    }

    public bool Load()
    {
        datas = sl.LoadData(typeof(Datas), "PlaySave/save.json", true) as Datas;
        if (datas == null)
            return false;
        SceneController.Singleton.OnFadeOutStart += () => StartCoroutine(LoadData());
        SceneController.Singleton.LoadSceneAsync(datas.playerData.rebirthSceneIndex, true);
        return true;
    }

    private IEnumerator LoadData()
    {
        // 先確保其他物件初始化完畢
        yield return null;
        SceneController.Singleton.OnFadeOutStart -= () => StartCoroutine(LoadData());
        if (Player.Singleton != null)
        {
            Player.Singleton.movement.canInput = false;
            Player.Singleton.transform.position = datas.playerData.position;
            Player.Singleton.transform.rotation = Quaternion.Euler(datas.playerData.rotation);
            Player.Singleton.transform.localScale = datas.playerData.scale;
            if (Player.Singleton.lifeSystem != null)
            {
                Player.Singleton.lifeSystem.IsInvincible = true;
                Player.Singleton.lifeSystem.SetStartPosition(datas.playerData.rebirthSceneIndex, datas.playerData.rebirthPosition);
                Player.Singleton.lifeSystem.NowHp = datas.playerData.nowHp;
                Player.Singleton.lifeSystem.InitializeHHeart();
                yield return null;
            }
            Player.Singleton.movement.canInput = true;
            Player.Singleton.lifeSystem.IsInvincible = false;
        }
        yield return null;
    }
}
