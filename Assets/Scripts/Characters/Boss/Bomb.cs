using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Characters;
using UnityEngine.UI;

public class Bomb : MonoBehaviour
{
    // 節拍單位
    public TempoActionType actionType;
    // 幾個節拍單位後爆炸
    public int tempoToExplosion;
    // 爆炸範圍
    public float radius;
    // 傷害
    public int damage;
    // 爆炸推力
    public float pushPower;
    // 推人距離
    public float pushDistance;
    // 可以被這個炸彈傷害或炸飛的物件的層
    public LayerMask layerMask;

    public RectTransform canvas;
    public Text countdownText;

    private SpriteRenderer spriteRenderer;
    public Sprite red;
    public Sprite black;
    public Transform hint;

    private int tempoCount;

    public new Rigidbody2D rigidbody;

    private CircleCollider2D collider2d;

    public delegate void BombEvent();
    public event BombEvent OnBomb;

    private IEnumerator enumeratorMove;

    private Vector2 settingDestination;
    private bool playerTouched;
    private bool bossTouched;
    private bool finishSetting;

    private bool isMoving;
    private bool isExplosion;

    private void Awake()
    {
        tempoCount = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider2d = GetComponent<CircleCollider2D>();
        hint.localScale = new Vector3(hint.localScale.x / transform.localScale.x * radius * 2, hint.localScale.y / transform.localScale.y * radius * 2, 1);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (countdownText != null)
            countdownText.text = tempoToExplosion.ToString();
    }

    private void Update()
    {
        canvas.localRotation = Quaternion.Euler(0, 0, -transform.localRotation.eulerAngles.z);
        // 炸彈被推後撞到東西立刻爆炸
        Collider2D hit2D = Physics2D.OverlapCircle(transform.position, collider2d.radius * transform.localScale.x, layerMask);
        if (hit2D != null && isMoving)
        {
            if (playerTouched)
            {
                if (hit2D.CompareTag("Enemy"))
                    Explosion();
                else if (hit2D.CompareTag("Boss"))
                    Explosion();
                else if (hit2D.CompareTag("Ground"))
                    Explosion();
            }
            else if (bossTouched)
            {
                if (hit2D.CompareTag("Player"))
                    Explosion();
                else if (hit2D.CompareTag("Ground"))
                    Explosion();
            }
        }
    }

    /// <summary>
    /// For boss, let bomb go to destination.
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="speed"></param>
    public void SetDestination(Vector2 destination, float speed)
    {
        finishSetting = false;
        playerTouched = false;
        bossTouched = false;
        settingDestination = destination;
        enumeratorMove = Move(speed);
        StartCoroutine(enumeratorMove);
    }

    /// <summary>
    /// Go to destination.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Move(float speed)
    {
        isMoving = true;
        while (Vector2.Distance(transform.position, settingDestination) > speed * Time.deltaTime)
        {
            transform.position = Vector2.MoveTowards(transform.position, settingDestination, speed * Time.deltaTime);
            yield return null;
        }
        isMoving = false;
        transform.position = settingDestination;
        finishSetting = true;
        ObjectTempoControl.Singleton.AddToBeatAction(Count, actionType);
        yield return null;
    }

    /// <summary>
    /// 計數
    /// </summary>
    private void Count()
    {
        if (isExplosion)
            return;
        tempoCount++;
        // 每一拍切圖片
        spriteRenderer.sprite = tempoCount % 2 == 0 ? red : black;
        // 倒計時數字
        if (countdownText != null)
            countdownText.text = (tempoToExplosion - tempoCount).ToString();
        // 時間到，爆炸
        if (tempoCount == tempoToExplosion)
            Explosion();
    }

    private void Explosion()
    {
        isExplosion = true;
        ObjectTempoControl.Singleton.RemoveToBeatAction(Count, actionType);
        gameObject.layer = LayerMask.GetMask("IgnoreRaycast");
        tempoCount = 0;
        // 取得爆炸範圍內的物件
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, radius, Vector2.zero, 0, layerMask);
        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.CompareTag("Player"))
                {
                    Vector2 direction = Player.Singleton.transform.position - transform.position;
                    if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
                        direction.y = 0;
                    else
                        direction.x = 0;
                    Player.Singleton.movement.Knock(direction, pushDistance, pushPower);
                    Player.Singleton.lifeSystem.Hurt(damage);
                }
                else if (hits[i].collider.CompareTag("Boss"))
                {
                    Debug.Log("BOSS");
                    hits[i].collider.GetComponentInParent<BigSquid>().Damaged(damage);
                }
                else if (hits[i].collider.CompareTag("Enemy"))
                {
                    hits[i].collider.GetComponent<Anemy>().Disappear();
                }
                else if (hits[i].collider.CompareTag("Bullet"))
                {
                    hits[i].collider.GetComponent<Bullet>().DestroyMyself();
                }
            }
        }
        OnBomb?.Invoke();
        Destroy(gameObject);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="direcion"></param>
    /// <param name="power"></param>
    /// <param name="toucher">玩家: 0, Boss: 1</param>
    public void Knock(Vector2 direcion, float power, int toucher)
    {
        if (toucher == 0)
            playerTouched = true;
        else if (toucher == 1)
            bossTouched = true;
        settingDestination = (Vector2)transform.position + direcion * 40.0f;
        enumeratorMove = Move(power);
        StartCoroutine(enumeratorMove);
    }

    public void Disappear()
    {
        ObjectTempoControl.Singleton.RemoveToBeatAction(Count, actionType);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            float pushedSpeed = Player.Singleton.movement.NowSpeed;
            if (pushedSpeed >= Player.Singleton.movement.moveSpeed)
            {
                Knock(Player.Singleton.movement.MoveDirection, pushedSpeed * 2.0f, 0);
                Player.Singleton.movement.Knock(Vector2.zero);
            }
            else
                Player.Singleton.movement.Knock(Vector2.zero);
        }
        if (collision.CompareTag("Player") || collision.CompareTag("Ground"))
        {
            if (!finishSetting)
            {
                Vector2 pos = transform.position;
                pos.x = Mathf.Floor(pos.x) + 0.5f;
                pos.y = Mathf.Floor(pos.y) + 0.5f;
                settingDestination = pos;
            }
        }
    }
}
