using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour
{

    SaveAndLoad SL;
    public Text playerName;
    public Text InputData;//在Inspector視窗中拖入InputField底下的Text

    public void saveButton()
    {
        playerDataType p = new playerDataType();
        string _playerName = InputData.text;
        p.name = _playerName;
        SL.SaveData(p);
    }
    public void LoadButton()
    {
        playerDataType p = (playerDataType)SL.LoadData(typeof(playerDataType));
        playerName.text = "Current Name:" + p.name;//載入時修改場景裡的資料
        Debug.Log(p);
    }
    void Start()
    {
        SL = GetComponent<SaveAndLoad>();
        LoadButton();
    }

}
public class playerDataType
{
    public string name = "player";
}