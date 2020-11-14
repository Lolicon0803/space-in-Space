using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Events;
using System.IO;
using System;
/// <summary>
/// Save And Load
/// 本篇腳本參考：http://zxxcc0001.pixnet.net/blog/post/243195373-unity---各平台檔案路徑
/// 腳本流程：按下存檔 -> 序列化存檔資料 -> 檢查資料夾是否存在 -> 將資料寫入文件
/// </summary>
//[RequireComponent(typeof(PlayerData))]
public class SaveAndLoad : MonoBehaviour
{

    //public string savingFileName = "GameData.txt";//存檔檔名
    private string nameAndPath;//存檔路徑+檔名

    /// <summary>
    /// 創建一個文件資料夾
    /// </summary>
    /// <param name="filePath">File name.</param>
    public void CreateDirectory(string filePath)
    {

        if (File.Exists(filePath))//如果資料夾位置已經存在則返回
            return;
        Directory.CreateDirectory(filePath);//新增資料夾 filePath為資料夾路徑
    }

    /// <summary>
    /// 序列化存檔資料
    /// </summary>
    /// <returns>The object.</returns>
    /// <param name="PlayerData">玩家保存的資料</param>
    private string SerializeObject(object PlayerData)
    {
        string serializePlayerData = "";
        serializePlayerData = JsonConvert.SerializeObject(PlayerData);//序列化玩家存檔
        return serializePlayerData;//返回字串型態的玩家存檔
    }

    /// <summary>
    /// Deserializes the object.
    /// </summary>
    /// <returns>The object.</returns>
    /// <param name="PlayerDataString">玩家保存資料</param>
    /// <param name="_PlayerDataType">保存資料的型別</param>
    private static object DeserializeObject(string _PlayerData, Type _PlayerDataType)
    {
        object playerData = null;
        playerData = JsonConvert.DeserializeObject(_PlayerData, _PlayerDataType);//反序列化玩家的存檔
        return playerData;//返回自定義類別的玩家存檔
    }

    /// <summary>
    /// 儲存檔案
    /// 資料參考：http://zxxcc0001.pixnet.net/blog/post/243195373-unity---各平台檔案路徑
    /// </summary>
    /// <param name="content">Content.</param>
    public void SaveData(object content, string savingFileName)
    {
        string content_string = SerializeObject(content);//序列化輸入資料
        string filePath = Application.dataPath + "/StreamingAssets" + "/Save";

        CreateDirectory(filePath);//在目的地新增資料夾
        nameAndPath = filePath + "/" + savingFileName;//存檔的位置加檔名

        StreamWriter _streamwriter = File.CreateText(nameAndPath);//新增存檔ㄒ
        _streamwriter.Write(content_string);//寫入存檔
        _streamwriter.Close();

    }
    public object LoadData(Type dataType, string savingFileName)
    {
        string filePath = Application.dataPath + "/StreamingAssets" + "/Save";
        nameAndPath = filePath + "/" + savingFileName;//存檔的位置加檔名

        StreamReader _streamReader = File.OpenText(nameAndPath);
        string data = _streamReader.ReadToEnd();//讀取所有存檔

        _streamReader.Close();//記得要關閉，不然會報錯
        return DeserializeObject(data, dataType);
    }
}