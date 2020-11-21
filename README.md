# Space in Space 開發

## 檔案目錄結構

-   Animations
    unity 動畫相關的檔案

-   Audios
    音樂相關檔案

-   Prefabs

-   Scenes

-   Scripts

-   Sprites
    各種圖片

## Scenes Build Order

-   0.Main Menu

-   1.CharacterScene 1

## Prefab Variable Description

-   SpaceJunk

| 名稱           | 型別  | 說明                                           |
| -------------- | ----- | ---------------------------------------------- |
| Route          | array | 路線設計，請參閱 Route                         |
| TempoType      | enum  | 三擇一，Whole、Helf、Quarter，此物件跟隨的節拍 |
| KnockDistance  | int   | 撞到主角時，主角會被撞飛幾格                   |
| KnockPower     | int   | 撞到主角時，主角飛的速度                       |
| MoveSpeed      | int   | 此物件本身的移動速度                           |
| IsGoStartPoint | bool  | Route 跑完後是否回到一開始的位置               |

-   Route

| 名稱      | 型別 | 說明                          |
| --------- | ---- | ----------------------------- |
| Size      | int  | 路線陣列尺寸                  |
| Direction | enum | 四擇一，上下左右              |
| Distance  | int  | 要往 Direction 的方向移動幾格 |

-   Anemy

| 名稱       | 型別     | 說明                                           |
| ---------- | -------- | ---------------------------------------------- |
| Route      | array    | 路線設計，請參閱 Route                         |
| TempoType  | enum     | 三擇一，Whole、Helf、Quarter，此物件跟隨的節拍 |
| BulletData | struture | 撞到主角時，主角會被撞飛幾格                   |
| MoveSpeed  | int      | 此物件本身的移動速度                           |

-   BulletData

| 名稱      | 型別 | 說明                          |
| --------- | ---- | ----------------------------- |
| Speed     | int  | 子彈速度                      |
| Direction | enum | 四擇一，上下左右              |
| Distance  | int  | 要往 Direction 的方向移動幾格 |

-   RazerMachine

| 名稱            | 型別   | 說明                                                             |
| --------------- | ------ | ---------------------------------------------------------------- |
| RazerDistance   | int    | 雷射長度，單位:格                                                |
| RazerDiractor   | int    | 雷射發射角度，共 360 度                                          |
| RazerTempo      | bool[] | 雷射行動模式                                                     |
| TempoType       | enum   | 三擇一，Whole、Helf、Quarter，此物件跟隨的節拍                   |
| WaitTempo       | int    | 此物件初始化完成後須要等幾個節拍再行動，用以調整跟其他物件的配合 |
| RazerWaitStatus | bool   | 雷射在等待期間是否為發射狀態(建議使用 false)                     |

-   BlackHole

| 名稱                | 型別  | 說明                               |
| ------------------- | ----- | ---------------------------------- |
| TempoType           | enum  | 節拍到吸一次人，動畫播放中都會吸人 |
| ImpactSpeed         | float | 玩家吸入黑洞的速度                 |
| ImpactRotationSpeed | float | 玩家旋轉速度                       |

-   WhiteHole

| 名稱       | 型別  | 說明                               |
| ---------- | ----- | ---------------------------------- |
| ActionType | enum  | 節拍到推一次人，動畫播放中都會推人 |
| PushUnit   | int   | 玩家會被推幾格                     |
| PushSpeed  | float | 玩家被推的速度                     |

-   Telepoter

| 名稱                | 型別      | 說明                                |
| ------------------- | --------- | ----------------------------------- |
| ActiveTempo         | enum      | 節拍到送一次人，動畫播放中都會送人  |
| IsEntrance          | bool      | 他是入口，prefab 已經勾好入口跟出口 |
| Exit                | Telepoter | 出口                                |
| SendTempo           | enum      | 節拍到把人送出去                    |
| IsExit              | bool      | 現在沒用                            |
| PushUnit            | int       | 出來後推人推多遠(設定在出口)        |
| PushSpeed           | float     | 出來後推多快(設定在出口)            |
| ImpactRotationSpeed | float     | 玩家轉多快進去入口                  |
| PushRotationSpeed   | float     | 玩家轉多快出來出口(設定在出口)      |
| PushDirction        | Vector2   | 出來後往哪推(設定在出口)            |

-   RebirthPointTrigger

| 名稱         | 型別    | 說明                     |
| ------------ | ------- | ------------------------ |
| RebirthPoint | Vector2 | 玩家碰到後要設定的復活點 |

-   HittingUI

| 名稱 | 型別 | 說明           |
| ---- | ---- | -------------- |
| Msg  | Text | 顯示訊息的文字 |

## Asset store

| 名稱                | 說明                                                                                                                                                                            |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| JSON .NET For Unity | **SaveAndLoad.cs** <br>SaveData(Type dataType) -> SaveData(Type dataType, string savingFileName) <br> LoadData(Type dataType) -> LoadData(Type dataType, string savingFileName) |
