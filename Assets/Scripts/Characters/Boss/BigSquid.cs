using Assets.Scripts.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// 第一隻大型魷魚BOSS
/// 攻擊模式:
///     1. 3 <= HP <= 10
///         * 每  2 拍召喚魷魚炸彈 1 顆，最多 7 顆
///         * 每  5 拍召喚小魷魚 1 隻，最多 3 隻
///         * 每 15 拍休息 2 拍，突刺觸手 2 下。
///     2. 0 <= HP <= 2
///         * 每 2 拍召喚魷魚炸彈 2 顆，最多 7 顆
///         * 每 5 拍召喚小魷魚 2 隻，最多 5 隻
///         * 每 15 拍休息 1 拍，突刺觸手 2 下。
///     3. HP == 6 || HP == 3
///         * 舉手 2 拍後大揮手 1 次。
/// </summary>
public class BigSquid : MonoBehaviour
{
    public int maxHP;
    private int nowHP;

    public Bomb bomb;
    public Anemy smallSquid;

    public Image hpBar;

    // 炸彈要召喚的範圍
    public float bombCircleInterRadius;
    public float bombCircleOuterRadius;
    // 小魷魚要召喚的範圍
    public float squidCircleInterRadius;
    public float squidCircleOuterRadius;

    // 半拍節奏計數
    private int halfCount;
    // 一拍節奏計數
    private int wholeCount;
    // 觸手攻擊計數
    private int attackCount;
    // 大揮手計數
    private int bigHandCount;

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

    private BigSquidHand[] hands;

    // 休息中
    private bool isResting;
    // 使用觸手
    private bool usingHand;
    // 每幾拍觸手攻擊
    private int handAfterTempo;
    // 觸手前休息時間
    private int restTimeBeforeHand;
    // 大揮手
    private bool useBigHand;
    // 死亡時呼叫，讓所有召喚物消失。
    public UnityAction OnDie;

    // 召喚界線
    public Vector2 summonBoundX = new Vector2(-7, 7);
    public Vector2 summonBoundY = new Vector2(-7, 7);

    private Animator animator;
    private AudioSource audioSource;

    // 動畫狀態機參數
    private readonly int animeIdleSpeed = Animator.StringToHash("IdleSpeed");
    private readonly int animeAttackHand = Animator.StringToHash("AttackHand");

    // 音效檔
    public AudioClip audioSummonBomb;
    public AudioClip audioSummonSquid;
    public AudioClip audioTouchPlayer;
    public AudioClip audioHurt;
    public AudioClip audioDie;
    // --

    private void Awake()
    {
        // 數據初始化--------
        nowHP = maxHP;
        halfCount = 0;
        wholeCount = 0;
        attackCount = 0;
        bigHandCount = 8;

        maxBombNumber = 7;
        nowBombNumber = 0;
        summonBombNumber = 1;

        maxSquidNumber = 3;
        nowSquidNumber = 0;
        summonSquidNumber = 1;

        isResting = false;
        usingHand = false;
        useBigHand = false;
        handAfterTempo = 15;
        restTimeBeforeHand = 2;
        //--------------------

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        hands = GetComponentsInChildren<BigSquidHand>();
    }

    // Start is called before the first frame update
    void Start()
    {
        OnDie = new UnityAction(() => { });
        animator.SetFloat(animeIdleSpeed, TempoManager.Singleton.beatPerMinute / 60.0f);
        SetActive();
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
    }

    private void CountWholeTempo()
    {
        if (useBigHand && !isResting)
        {
            bigHandCount++;
            animator.SetInteger(animeAttackHand, bigHandCount);
            if (bigHandCount == 12)
            {
                bigHandCount = 8;
                useBigHand = false;
                foreach (BigSquidHand hand in hands)
                    hand.EndAttack();
            }
        }
        else if (!isResting)
        {
            // 一拍計數
            wholeCount++;
            // 每兩拍且炸彈數量未達上限
            if (wholeCount % 2 == 0 && nowBombNumber < maxBombNumber)
                SummonBomb(summonBombNumber);

            // 每三拍且小魷魚數量未達上限
            if (wholeCount % 3 == 0 && nowSquidNumber < maxSquidNumber)
                SummonSquid(summonSquidNumber);

            // 抵達開始使用觸手拍點
            if (wholeCount % handAfterTempo == 0)
                isResting = true;
        }
        // 休息中
        else
        {
            // 計數增加
            attackCount++;
            // 達休息時間，下一拍結束後發射雷射
            if (attackCount == restTimeBeforeHand + 1 || usingHand)
            {
                foreach (BigSquidHand hand in hands)
                    hand.StartAttack();
                usingHand = true;
                int count = animator.GetInteger(animeAttackHand) + 1;
                animator.SetInteger(animeAttackHand, count);
                if (count == 7)
                {
                    usingHand = false;
                    attackCount = 0;
                    animator.SetInteger(animeAttackHand, 0);
                    isResting = false;
                    foreach (BigSquidHand hand in hands)
                        hand.EndAttack();
                }
            }
        }
    }

    public void SetHandKnockDirection(GameData.Direction direction)
    {
        Vector2 d = Vector2.zero;
        switch (direction)
        {
            case GameData.Direction.UP:
                d = Vector2.up;
                break;
            case GameData.Direction.DOWN:
                d = Vector2.down;
                break;
            case GameData.Direction.LEFT:
                d = Vector2.left;
                break;
            case GameData.Direction.RIGHT:
                d = Vector2.right;
                break;
        }
        foreach (BigSquidHand hand in hands)
            hand.SetKnockDirection(d);
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

    public void Damaged(int number)
    {
        if (audioHurt != null)
            audioSource.PlayOneShot(audioHurt);
        nowHP -= number;
        if (hpBar != null)
            hpBar.fillAmount = (float)nowHP / maxHP;
        if (nowHP == 6 || nowHP == 3)
        {
            useBigHand = true;
            foreach (BigSquidHand hand in hands)
                hand.StartAttack();
        }
        if (nowHP < 4)
        {
            summonBombNumber = 2;
            maxSquidNumber = 5;
            summonSquidNumber = 2;
            handAfterTempo = 10;
            restTimeBeforeHand = 1;
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
