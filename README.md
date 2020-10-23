# Space in Space 開發

## 檔案目錄結構
需要就往下加，要改的就再討論吧@@
* Animations
    #### unity動畫相關的檔案
* Audios
    #### 音樂相關檔案
* Prefabs
    #### 就prefab
* Scenes
    #### unity的scene檔案
* Scripts
    #### .C#檔
* Sprites
    #### 各種圖片
    
## 地圖製作教學
* SpaceJunk

|名稱|型別|說明|
|-----|--------|-|
|Route|array|路線設計，請參閱Route|
|TempoType |enum|三擇一，Whole、Helf、Quarter，此物件跟隨的節拍 |
|KnockDistance |int| 撞到主角時，主角會被撞飛幾格|
|KnockPower|int| 撞到主角時，主角飛的速度|
|MoveSpeed|int| 此物件本身的移動速度|
|IsGoStartPoint|bool|Route跑完後是否回到一開始的位置 |


* Route

|名稱|型別|說明|
|-----|--------|-|
|Size|int|路線陣列尺寸|
|Direction |enum|四擇一，上下左右 |
|Distance |int| 要往Direction的方向移動幾格|


* Anemy

|名稱|型別|說明|
|-----|--------|-|
|Route|array|路線設計，請參閱Route|
|TempoType |enum|三擇一，Whole、Helf、Quarter，此物件跟隨的節拍 |
|BulletData |struture| 撞到主角時，主角會被撞飛幾格|
|MoveSpeed|int| 此物件本身的移動速度|

* BulletData

|名稱|型別|說明|
|-----|--------|-|
|Speed|int|子彈速度|
|Direction |enum|四擇一，上下左右 |
|Distance |int| 要往Direction的方向移動幾格|


* RazerMachine

|名稱|型別|說明|
|-----|--------|-|
|RazerDistance|int|雷射長度，單位:格|
|RazerDiractor |int|雷射發射角度，共360度 |
|RazerTempo |bool[]| 雷射行動模式|
|TempoType|enum| 三擇一，Whole、Helf、Quarter，此物件跟隨的節拍|
|WaitTempo|int| 此物件初始化完成後須要等幾個節拍再行動，用以調整跟其他物件的配合|
|RazerWaitStatus|bool|雷射在等待期間是否為發射狀態(建議使用false) |
