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

    private int tempoCount;

    public new Rigidbody2D rigidbody;

    private CircleCollider2D collider;

    public delegate void BombEvent();
    public event BombEvent OnBomb;

    private IEnumerator enumeratorMove;

    private bool playerTouched;
    private bool bossTouched;
    private bool finishSetting;

    private void Awake()
    {
        tempoCount = 0;
        collider = GetComponent<CircleCollider2D>();
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
        Collider2D hit2D = Physics2D.OverlapCircle(transform.position, collider.radius * transform.localScale.x, layerMask); 
        if (hit2D != null)
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
        rigidbody.velocity = (destination - (Vector2)transform.position).normalized * speed;
        enumeratorMove = Move(destination, speed);
        StartCoroutine(enumeratorMove);
    }

    /// <summary>
    /// Go to destination.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Move(Vector2 destination, float speed)
    {
        while (Vector2.Distance(transform.position, destination) > 1)
            yield return null;
        rigidbody.velocity = Vector2.zero;
        finishSetting = true;
        ObjectTempoControl.Singleton.AddToBeatAction(Count, actionType);
        yield return null;
    }

    private void Count()
    {
        tempoCount++;
        if (countdownText != null)
            countdownText.text = (tempoToExplosion - tempoCount).ToString();
        if (tempoCount == tempoToExplosion)
            Explosion();
    }

    private void Explosion()
    {
        ObjectTempoControl.Singleton.RemoveToBeatAction(Count, actionType);
        gameObject.layer = LayerMask.GetMask("IgnoreRaycast");
        rigidbody.velocity = Vector2.zero;
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
                    Player.Singleton.movement.Knock(direction, pushDistance, pushPower);
                    Player.Singleton.lifeSystem.Hurt(damage);
                }
                else if (hits[i].collider.CompareTag("Boss"))
                {
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
        rigidbody.velocity = direcion * power;
    }

    public void Disappear()
    {
        ObjectTempoControl.Singleton.RemoveToBeatAction(Count, actionType);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            float pushedSpeed = Player.Singleton.movement.NowSpeed;
            if (pushedSpeed >= Player.Singleton.movement.moveSpeed)
            {
                Knock(Player.Singleton.movement.MoveDirection, pushedSpeed, 0);
                Player.Singleton.movement.Knock(Vector2.zero);
            }
        }
        if (!finishSetting)
        {
            StopCoroutine(enumeratorMove);
            finishSetting = true;
            ObjectTempoControl.Singleton.AddToBeatAction(Count, actionType);
        }
    }

}
