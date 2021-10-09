using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[System.Serializable]
public class PlayerData
{
    public Vector2 position;
    public Vector2 rotation;
    public Vector2 scale;
    public int nowHp;
    public int rebirthSceneIndex;
    public Vector2 rebirthPosition;

    //public void SaveDataWhenSceneChange(Vector2 nextStartPosition)
    //{
    //    data.position = Player.Singleton.transform.position;
    //    data.rotation = Player.Singleton.transform.rotation;
    //    data.scale = Player.Singleton.transform.localScale;
    //    data.nowHp = Player.Singleton.lifeSystem.NowHp;
    //    data.nowSpeed = Player.Singleton.movement.NowSpeed;
    //    data.vectorToMovePoint = Player.Singleton.movement.movePoint - data.position;
    //    data.recoverCount = Player.Singleton.lifeSystem.RecoverCount;
    //    data.isDie = Player.Singleton.lifeSystem.IsDie;
    //    data.rebirthSceneIndex = Player.Singleton.lifeSystem.startSceneIndex;
    //    data.rebirthPosition = Player.Singleton.lifeSystem.startPosition;
    //    data.nextSceneStartPosition = nextStartPosition;
    //}

    //public static void SaveButton()
    //{
    //    SL = new SaveAndLoad();
    //    //PlayerDataType p = new PlayerDataType();
    //    //string _playerName = InputData.text;
    //    //p.name = _playerName;
    //   string s = JsonUtility.ToJson(Player.Singleton.movement);
    //    Debug.Log(s);
    //    SL.SaveData(Player.Singleton.movement, "GameData.txt");
    //}
    //public void LoadButton()
    //{
    //    PlayerDataType p = (PlayerDataType)SL.LoadData(typeof(PlayerDataType), "GameData.txt");
    //    //playerName.text = "Current Name:" + p.name;//載入時修改場景裡的資料
    //    Debug.Log(p);
    //}
    //void Start()
    //{
    //    SL = GetComponent<SaveAndLoad>();
    //    LoadButton();
    //}

}

public class PlayerDataType
{
    //public Vector2 position;
    //public Quaternion rotation;
    //public Vector2 scale;
    //// 是否可以輸入
    //public bool canInput;
    //// 現在血量
    //public int nowHp;
    //// 現在速度
    //public float nowSpeed;
    //// 與移動點差距
    //public Vector2 vectorToMovePoint;
    //// 回復計數
    //public int recoverCount;
    //// 是否是死亡狀態
    //public bool isDie;
    //// 復活場景
    //public int rebirthSceneIndex;
    //public Vector2 rebirthPosition;
    //// 切場景後的初始位置
    //public Vector2 nextSceneStartPosition;
}