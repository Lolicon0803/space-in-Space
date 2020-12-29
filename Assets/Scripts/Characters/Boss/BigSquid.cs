using Assets.Scripts.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// 第一隻大型魷魚BOSS
/// 攻擊模式:
///     1. 5 <= HP <= MAXHP
///        * 每 1 拍在玩家周圍召喚 1 顆炸彈，最多 7 顆。
///        * 每 6 拍在自己前方召喚 1 隻小魷魚，最多 2 隻，
///        * 每 3 拍連續直線攻擊 2 次。
///        * 每 10 拍扇形攻擊 1 次。
///     2. HP <= 4
///        * 每 2 拍在玩家周圍召喚 1 顆炸彈，最多 9 顆。
///        * 每 6 拍在自己前方召喚 1 隻小魷魚，最多 3 隻，
///        * 每 3 拍連續直線攻擊 3 次。
///        * 每 10 拍扇形攻擊 1 次。
/// </summary>
public class BigSquid : MonoBehaviour
{
    public int maxHP;
    public int NowHP { get; private set; }

    public Bomb bomb;
    public Anemy smallSquid;

    public Canvas hpCanvas;
    public Image hpBar;

    // 召喚界線
    public Vector2 bombSummonBoundX = new Vector2(-7, 7);
    public Vector2 bombSummonBoundY = new Vector2(-7, 7);
    // 炸彈要召喚的範圍
    private int bombSummonDistance = 5;
    // 小魷魚要召喚的範圍
    private float squidSummonX = 1.5f;
    private Vector2 squidSummonY = new Vector2(-6.5f, 6.5f);

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

    // 是否正在觸手直線攻擊
    private bool isAttackingStraight;
    // 攻擊次數
    private int attackStraightNumber;
    // 一次打幾次
    private int maxStraightNumber;
    // 是否正在扇形攻擊
    private bool isAttackingSector;

    [HideInInspector]
    public BigSquidHand hand;

    // 死亡時呼叫，讓所有召喚物消失。
    public UnityAction OnDie;

    private Animator animator;
    private AudioSource audioSource;

    // 動畫狀態機參數
    private readonly int animeIdleSpeed = Animator.StringToHash("IdleSpeed");
    private readonly int animeAttackHand = Animator.StringToHash("AttackHand");

    // 音效檔
    public AudioClip audioTouchPlayer;
    public AudioClip audioHurt;
    public AudioClip audioDie;
    // --

    // 特殊行為，連續用觸手推動炸彈
    private bool straightBombAttack;
    private int straightBombAttackCount;
    private Vector2 targetBombPos;

    private Transform dieBomb;

    private void Awake()
    {
        // 數據初始化--------
        NowHP = maxHP;
        wholeCount = 0;
        attackCount = 0;
        bigHandCount = 0;

        maxBombNumber = 9;
        nowBombNumber = 0;
        summonBombNumber = 2;

        maxSquidNumber = 2;
        nowSquidNumber = 0;
        summonSquidNumber = 1;

        isAttackingStraight = false;
        attackStraightNumber = 0;
        maxStraightNumber = 2;

        isAttackingSector = false;

        straightBombAttack = false;
        straightBombAttackCount = 0;
        //--------------------

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        hand = GetComponentInChildren<BigSquidHand>();
        dieBomb = transform.GetChild(3);
    }

    // Start is called before the first frame update
    void Start()
    {
        OnDie = new UnityAction(() => { });
        OnDie += hand.RemoveTempoEvent;
    }

    public void SetActive()
    {
        //// 註冊半拍節奏
        //ObjectTempoControl.Singleton.AddToBeatAction(CountHalfTempo, TempoActionType.Half);
        //// 註冊一拍節奏
        ObjectTempoControl.Singleton.AddToBeatAction(CountWholeTempo, TempoActionType.Whole);
        ObjectTempoControl.Singleton.AddToBeatAction(UseStraightBombAttack, TempoActionType.Half);
    }

    private void CountWholeTempo()
    {
        if (straightBombAttack && !isAttackingStraight)
            return;
        else if (!isAttackingStraight && !isAttackingSector)
        {
            // 一拍計數
            wholeCount++;
            // 每 6 拍，可以放魷魚的話會先放魷魚
            if (wholeCount % 6 == 0 && nowSquidNumber < maxSquidNumber)
                SummonSquid(summonSquidNumber);
            // 扇形
            else if (wholeCount % 11 == 0)
                isAttackingSector = true;
            // 直線
            else if (wholeCount % 3 == 0)
                isAttackingStraight = true;
            // 炸彈
            if (nowBombNumber < maxBombNumber)
                SummonBomb(summonBombNumber);
        }
        else if (isAttackingStraight)
        {
            attackCount = (attackCount + 1) % 3;
            hand.RaiseForStraight(attackCount);
            if (attackCount == 0)
            {
                attackStraightNumber++;
                if (attackStraightNumber == maxStraightNumber)
                {
                    attackStraightNumber = 0;
                    isAttackingStraight = false;
                }
            }
        }
        else if (isAttackingSector)
        {
            bigHandCount++;
            if (bigHandCount == 1)
                hand.RaiseForSetor(1);
            else if (bigHandCount == 5)
                hand.RaiseForSetor(2);
            else if (bigHandCount >= 7)
            {
                hand.RaiseForSetor(0);
                bigHandCount = 0;
                isAttackingSector = false;
            }
        }
    }

    private void UseStraightBombAttack()
    {
        if (!straightBombAttack)
            return;
        straightBombAttackCount++;
        if (straightBombAttackCount % 2 == 1)
        {
            targetBombPos = Player.Singleton.transform.position;
            targetBombPos.x = -1.5f;
            SummonBombAt(targetBombPos);
            hand.RaiseForStraight(0);
            hand.RaiseForStraight(1, false);
            hand.SetAttackPosition(targetBombPos);
        }
        else
        {
            hand.RaiseForStraight(2);
        }
        if (straightBombAttackCount == 20)
        {
            straightBombAttack = false;
            ObjectTempoControl.Singleton.RemoveToBeatAction(UseStraightBombAttack, TempoActionType.Half);
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
            // 取得召喚位置並確保在格子內
            Vector2 position = new Vector2(Mathf.Floor(squidSummonX) + 0.5f, Mathf.Floor(Random.Range(squidSummonY.x, squidSummonY.y)) + 0.5f);
            Anemy newSquid = Instantiate(smallSquid, transform.position, Quaternion.identity);
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
            // 取得召喚位置並確保在格子內
            Vector2 position = Player.Singleton.transform.position;
            float leftX = position.x - bombSummonDistance;
            float rightX = position.x + bombSummonDistance;
            float downY = position.y - bombSummonDistance;
            float upY = position.y + bombSummonDistance;
            if (leftX < bombSummonBoundX.x)
                leftX = bombSummonBoundX.x;
            if (rightX > bombSummonBoundX.y)
                rightX = bombSummonBoundX.y;
            if (downY < bombSummonBoundY.x)
                downY = bombSummonBoundY.x;
            if (upY > bombSummonBoundY.y)
                upY = bombSummonBoundY.y;
            position.x = Mathf.Floor(Random.Range(leftX, rightX)) + 0.5f;
            position.y = Mathf.Floor(Random.Range(downY, upY)) + 0.5f;
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
            newBomb.gameObject.SetActive(true);
            newBomb.SetDestination(position, 20 * TempoManager.Singleton.beatPerMinute / 60.0f);
        }
    }

    private void SummonBombAt(Vector2 position)
    {
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
        newBomb.gameObject.SetActive(true);
        newBomb.SetDestination(position, 40 * TempoManager.Singleton.beatPerMinute / 60.0f);
    }

    public void Damaged(int number)
    {
        if (audioHurt != null)
            audioSource.PlayOneShot(audioHurt);
        NowHP -= number;
        if (hpBar != null)
            hpBar.fillAmount = (float)NowHP / maxHP;
        //if (nowHP == 10)
        //{
        //    Debug.Break();
        //    straightBombAttack = true;
        //    attackStraightNumber = maxStraightNumber;
        //    ObjectTempoControl.Singleton.AddToBeatAction(UseStraightBombAttack, TempoActionType.Quarter);
        //}
        if (NowHP < 4)
        {
            summonBombNumber = 3;
            maxBombNumber = 12;
            maxSquidNumber = 3;
            maxStraightNumber = 3;
        }
        if (NowHP <= 0)
        {
            ObjectTempoControl.Singleton.RemoveToBeatAction(CountWholeTempo, TempoActionType.Whole);
            if (audioDie != null)
                audioSource.PlayOneShot(audioDie);
            OnDie?.Invoke();
            NowHP = -1;
        }
    }

    public IEnumerator ShowDieEffect()
    {
        yield return new WaitForSeconds(0.5f);
        dieBomb.GetChild(0).GetComponent<Animator>().SetTrigger("Explosion");
        yield return new WaitForSeconds(0.1f);
        dieBomb.GetChild(1).GetComponent<Animator>().SetTrigger("Explosion");
        yield return new WaitForSeconds(0.5f);
        dieBomb.GetChild(2).GetComponent<Animator>().SetTrigger("Explosion");
        yield return new WaitForSeconds(0.6f);
        dieBomb.GetChild(3).GetComponent<Animator>().SetTrigger("Explosion");
        hand.GetComponent<SpriteRenderer>().sprite = hand.dieLie;
        yield return new WaitForSeconds(0.5f);
        dieBomb.GetChild(4).GetComponent<Animator>().SetTrigger("Explosion");
        yield return new WaitForSeconds(0.45f);
        dieBomb.GetChild(5).GetComponent<Animator>().SetTrigger("Explosion");
        yield return new WaitForSeconds(0.25f);
        dieBomb.GetChild(6).GetComponent<Animator>().SetTrigger("Explosion");
        yield return new WaitForSeconds(0.65f);
        dieBomb.GetChild(7).GetComponent<Animator>().SetTrigger("Explosion");
        yield return new WaitWhile(() => dieBomb.GetChild(7).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length < 0.99f);
        NowHP = -1;
        Destroy(hpCanvas.gameObject);
        Destroy(gameObject);
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
        //Gizmos.color = Color.red;
        //for (int i = 0; i < 5000; i++)
        //{
        //    Vector2 p = Random.insideUnitCircle * (bombCircleOuterRadius - bombCircleInterRadius);
        //    p = p.normalized * (bombCircleInterRadius + p.magnitude);
        //    p.x = Mathf.Clamp(p.x, summonBoundX.x, summonBoundX.y);
        //    p.y = Mathf.Clamp(p.y, summonBoundY.x, summonBoundY.y);
        //    Gizmos.DrawSphere(p, 0.1f);
        //}

        Vector2 luc = new Vector2(bombSummonBoundX.x, bombSummonBoundY.y);
        Vector2 ruc = new Vector2(bombSummonBoundX.y, bombSummonBoundY.y);
        Vector2 rdc = new Vector2(bombSummonBoundX.y, bombSummonBoundY.x);
        Vector2 ldc = new Vector2(bombSummonBoundX.x, bombSummonBoundY.x);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(luc, ruc);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(ruc, rdc);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(rdc, ldc);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(ldc, luc);

        //Gizmos.color = Color.green;
        //for (int i = 0; i < 2000; i++)
        //{
        //    Vector2 p = Random.insideUnitCircle * (squidCircleOuterRadius - squidCircleInterRadius);
        //    p = p.normalized * (squidCircleInterRadius + p.magnitude);
        //    p.x = Mathf.Clamp(p.x, summonBoundX.x, summonBoundX.y);
        //    p.y = Mathf.Clamp(p.y, summonBoundY.x, summonBoundY.y);
        //    Gizmos.DrawSphere(p, 0.1f);
        //}
    }
}
