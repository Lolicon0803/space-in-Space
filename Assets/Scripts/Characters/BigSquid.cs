using Assets.Scripts.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// 第一隻大型魷魚BOSS
/// 攻擊模式:
///     1. 3 <= HP <= 7
///         * 每 0.5 拍移動一格
///         * 每 2 拍召喚魷魚炸彈 1 顆，最多 7 顆
///         * 每 5 拍召喚小魷魚 1 隻，最多 3 隻
///         * 每移動 20 次休息 2 拍，timeout 時發射雷射3發持續 3 拍。
///     2. 1 <= HP <= 2
///         * 每 0.5 拍移動一格，速度加快
///         * 每 2 拍召喚魷魚炸彈 2 顆，最多 7 顆
///         * 每 5 拍召喚小魷魚 2 隻，最多 5 隻
///         * 每移動 15 次休息 2 拍，timeout時發射雷射持續 3 拍。
///   --3. HP = 1
///         * 召喚白洞到正中間     
/// </summary>
public class BigSquid : MonoBehaviour, IObjectBehavier
{
    public int maxHP;
    private int nowHP;

    public Bomb bomb;
    public Anemy smallSquid;
    public Razer razer1;
    public Razer razer2;
    public Razer razer3;

    public float moveSpeed;
    //使用者輸入用
    public GameData.RouteData[] route;
    //迴圈讀取用
    private List<GameData.Direction> routeMap = new List<GameData.Direction>();

    public Image hpBar;

    // 炸彈要召喚的範圍
    public float bombCircleInterRadius;
    public float bombCircleOuterRadius;
    // 小魷魚要召喚的範圍
    public float squidCircleInterRadius;
    public float squidCircleOuterRadius;

    private int routeIndex = 0;
    // 下一個移動點
    private Vector2 movePoint;
    // 移動方向
    private Vector2 moveDirection;

    // 半拍節奏計數
    private int halfCount;
    // 一拍節奏計數
    private int wholeCount;
    // 移動計數
    private int moveCount;
    // 使用雷射
    private bool usingRazer;
    // 炸彈最多數量
    private int maxBombNumber;
    // 現在炸彈數量
    private int nowBombNumber;
    // 炸彈一次召喚數量
    private int summonBombNumber;
    // 小魷魚最多數量
    private int maxSquidNumber;
    // 現在魷魚數量
    private int nowSquidNumber;
    // 魷魚一次召喚數量
    private int summonSquidNumber;
    // 休息中
    private bool isResting;
    // 移動幾拍後雷射
    private int razerAfterMoveTime;
    // 雷射前休息時間
    private int restTimeBeforeRazer;
    // 雷射最多持續時間
    private int maxRazerTime;
    // 死亡時呼叫，讓所有召喚物消失。
    public UnityAction OnDie;

    // 召喚界線
    public Vector2 summonBoundX = new Vector2(-7, 7);
    public Vector2 summonBoundY = new Vector2(-7, 7);

    private AudioSource audioSource;

    // 音效檔
    public AudioClip audioSummonBomb;
    public AudioClip audioSummonSquid;
    public AudioClip audioTouchPlayer;
    public AudioClip audioHurt;
    public AudioClip audioPrepareLazer;
    public AudioClip audioShootLazer;
    public AudioClip audioDie;
    // --

    private void Awake()
    {
        // 數據初始化--------
        nowHP = maxHP;
        halfCount = 0;
        wholeCount = 0;
        moveCount = 0;

        maxBombNumber = 7;
        nowBombNumber = 0;
        summonBombNumber = 1;

        maxSquidNumber = 3;
        nowSquidNumber = 0;
        summonSquidNumber = 1;

        isResting = false;
        usingRazer = false;
        razerAfterMoveTime = 20;
        restTimeBeforeRazer = 2;
        maxRazerTime = 3;
        //--------------------

        audioSource = GetComponent<AudioSource>();

        // 獲得路線
        foreach (GameData.RouteData item in route)
        {
            for (int i = 0; i < item.distance; i++)
            {
                routeMap.Add(item.direction);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        OnDie = new UnityAction(() => { });
    }

    public void SetActive()
    {
        // 註冊半拍節奏
        ObjectTempoControl.Singleton.AddToBeatAction(CountHalfTempo, TempoActionType.Half);
        // 註冊一拍節奏
        ObjectTempoControl.Singleton.AddToBeatAction(CountWholeTempo, TempoActionType.Whole);
    }

    /// <summary>
    /// 半拍計數。
    /// </summary>
    private void CountHalfTempo()
    {
        if (isResting)
            return;

        // 半拍計數
        halfCount++;

        // 移動
        if (routeMap.Count > 0 && !isResting)
            StartCoroutine(Move());
    }

    private void CountWholeTempo()
    {
        if (!isResting)
        {
            // 一拍計數
            wholeCount++;

            // 每兩拍且炸彈數量未達上限
            if (wholeCount % 2 == 0 && nowBombNumber < maxBombNumber)
                SummonBomb(summonBombNumber);

            // 每三拍且小魷魚數量未達上限
            if (wholeCount % 3 == 0 && nowSquidNumber < maxSquidNumber)
                SummonSquid(summonSquidNumber);
        }
        // 休息中
        else
        {
            // 計數增加
            moveCount++;
            if (!usingRazer)
            {
                if (audioPrepareLazer != null)
                {
                    audioSource.clip = audioPrepareLazer;
                    audioSource.PlayOneShot(audioPrepareLazer);
                }
            }
            // 達休息時間，下一拍結束後發射雷射
            if (moveCount == restTimeBeforeRazer + 1 && !usingRazer)
            {
                audioSource.clip = null;
                audioSource.Stop();
                usingRazer = true;
                moveCount = 0;
                StartCoroutine(ControlRazer());
            }
        }
    }

    /// <summary>
    /// 召喚魷魚。
    /// </summary>
    private void SummonSquid(int number)
    {
        for (int i = 0; i < number; i++)
        {
            nowSquidNumber++;
            // 計算外圓
            Vector2 position = Random.insideUnitCircle * (squidCircleOuterRadius - squidCircleInterRadius);
            // 排除內圓
            position = position.normalized * (squidCircleInterRadius + position.magnitude);
            // 切掉地圖下限
            while (position.x <= summonBoundX.x || position.x >= summonBoundX.y || position.y <= summonBoundY.x || position.y >= summonBoundY.y)
            {
                position = Random.insideUnitCircle * (squidCircleOuterRadius - squidCircleInterRadius);
                position = position.normalized * (squidCircleInterRadius + position.magnitude);
            }
            Anemy newSquid = Instantiate(smallSquid, transform.position, Quaternion.identity);
            newSquid.bulletData.direction = (GameData.Direction)Random.Range(3, 5);
            newSquid.OnDisappear += () => nowSquidNumber--;
            // 死亡時，刪除這隻小魷魚
            OnDie += () =>
            {
                if (newSquid != null)
                {
                    newSquid.Disappear();
                    nowSquidNumber--;
                }
            };
            newSquid.gameObject.SetActive(true);
            if (audioSummonSquid != null)
                audioSource.PlayOneShot(audioSummonSquid);
            newSquid.SetDestination(position, 25);
        }
    }

    /// <summary>
    /// 召喚炸彈
    /// </summary>
    /// <param name="number">一次召喚幾顆</param>
    private void SummonBomb(int number)
    {
        for (int i = 0; i < number; i++)
        {
            nowBombNumber++;
            // 計算外圓
            Vector2 position = Random.insideUnitCircle * (bombCircleOuterRadius - bombCircleInterRadius);
            // 排除內圓
            position = position.normalized * (bombCircleInterRadius + position.magnitude);
            // 切掉地圖下限
            while (position.x <= summonBoundX.x || position.x >= summonBoundX.y || position.y <= summonBoundY.x || position.y >= summonBoundY.y)
            {
                position = Random.insideUnitCircle * (squidCircleOuterRadius - squidCircleInterRadius);
                position = position.normalized * (squidCircleInterRadius + position.magnitude);
            }
            Debug.DrawLine(transform.position, position, Color.blue, 2);
            Bomb newBomb = Instantiate(bomb, transform.position, Quaternion.identity);
            // 炸彈爆炸時減少炸彈數量。
            newBomb.OnBomb += () => nowBombNumber--;
            // 死亡時，刪除這顆炸彈。
            OnDie += () =>
            {
                if (newBomb != null)
                {
                    newBomb.Disappear();
                    nowBombNumber--;
                }
            };
            if (audioSummonBomb != null)
                audioSource.PlayOneShot(audioSummonBomb);
            newBomb.gameObject.SetActive(true);
            newBomb.SetDestination(position, 20);
        }
    }

    /// <summary>
    /// 控制雷射
    /// </summary>
    /// <returns></returns>
    private IEnumerator ControlRazer()
    {
        // 計算玩家位置
        Vector2 target = Vector2.down;
        if (audioShootLazer != null)
        {
            audioSource.clip = audioShootLazer;
            audioSource.loop = true;
            audioSource.PlayOneShot(audioShootLazer);
        }
        razer1.SetSize(25, 0.2f);
        razer1.transform.Rotate(Vector3.forward, Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg);
        razer2.SetSize(25, 0.2f);
        razer2.transform.Rotate(Vector3.forward, Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg + 45);
        razer3.SetSize(25, 0.2f);
        razer3.transform.Rotate(Vector3.forward, Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg - 45);
        // 持續三拍
        while (moveCount <= maxRazerTime)
            yield return null;
        // 關閉
        razer1.SetSize(0, 0);
        razer2.SetSize(0, 0);
        razer3.SetSize(0, 0);
        if (audioShootLazer != null)
        {
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.loop = false;
        }
        razer1.transform.localRotation = Quaternion.identity;
        razer2.transform.localRotation = Quaternion.identity;
        razer3.transform.localRotation = Quaternion.identity;
        isResting = false;
        usingRazer = false;
        moveCount = 0;
    }

    public IEnumerator Move()
    {
        //確認方向
        moveDirection = GameData.Map.directionMap[(int)routeMap[routeIndex]];
        // 下個移動點 + 朝路徑移動1格vector
        movePoint = (Vector2)transform.position + moveDirection;
        //移動
        while (Vector2.Distance(transform.position, movePoint) > moveSpeed * Time.deltaTime)
        {
            transform.position = Vector2.MoveTowards(transform.position, movePoint, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = movePoint;
        routeIndex = (routeIndex + 1) % routeMap.Count;

        moveCount++;
        if (moveCount == razerAfterMoveTime)
        {
            moveCount = 0;
            isResting = true;
        }
    }

    public void Damaged(int number)
    {
        if (audioHurt != null)
            audioSource.PlayOneShot(audioHurt);
        nowHP -= number;
        if (hpBar != null)
            hpBar.fillAmount = (float)nowHP / maxHP;
        if (nowHP < 4)
        {
            moveSpeed *= 2;
            summonBombNumber = 2;
            maxSquidNumber = 5;
            summonSquidNumber = 2;
            razerAfterMoveTime = 15;
        }
        if (nowHP <= 0)
        {
            ObjectTempoControl.Singleton.RemoveToBeatAction(CountHalfTempo, TempoActionType.Half);
            ObjectTempoControl.Singleton.RemoveToBeatAction(CountWholeTempo, TempoActionType.Whole);
            if (audioDie != null)
                audioSource.PlayOneShot(audioDie);
            OnDie?.Invoke();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (audioTouchPlayer != null)
                audioSource.PlayOneShot(audioTouchPlayer);
            Player.Singleton.movement.Knock(Vector2.zero, 3, 20);
            Player.Singleton.lifeSystem.Hurt(1);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < 2000; i++)
        {
            Vector2 p = Random.insideUnitCircle * (bombCircleOuterRadius - bombCircleInterRadius);
            p = p.normalized * (bombCircleInterRadius + p.magnitude);
            p.x = Mathf.Clamp(p.x, summonBoundX.x, summonBoundX.y);
            p.y = Mathf.Clamp(p.y, summonBoundY.x, summonBoundY.y);
            Gizmos.DrawSphere(p, 0.1f);
        }

        Gizmos.color = Color.green;
        for (int i = 0; i < 2000; i++)
        {
            Vector2 p = Random.insideUnitCircle * (squidCircleOuterRadius - squidCircleInterRadius);
            p = p.normalized * (squidCircleInterRadius + p.magnitude);
            p.x = Mathf.Clamp(p.x, summonBoundX.x, summonBoundX.y);
            p.y = Mathf.Clamp(p.y, summonBoundY.x, summonBoundY.y);
            Gizmos.DrawSphere(p, 0.1f);
        }
    }

}
