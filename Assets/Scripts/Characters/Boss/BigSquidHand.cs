using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigSquidHand : MonoBehaviour
{
    public float leftX;
    public float rightX;
    public float downY;
    public float upY;

    public SpriteRenderer hint1;
    public SpriteRenderer hintSector;

    private Animator animator;

    private Vector2 attackGrid1;

    private LayerMask hitableLayer;

    private int damage = 1;
    private int knockSpeed = 10;
    private int knockDistance = 3;

    private readonly int animStraight = Animator.StringToHash("Straight");
    private readonly int animSector = Animator.StringToHash("Sector");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        attackGrid1 = Vector2.zero;
        hitableLayer = LayerMask.GetMask("Player", "Bomb");
        // 校正縮放
        hint1.transform.localScale = new Vector3(1 / transform.parent.localScale.x, 1 / transform.parent.localScale.y, 1);
    }

    private void Start()
    {
        GetComponentInParent<BigSquid>().OnDie += RemoveTempoEvent;
    }

    public void RaiseForStraight(int number)
    {
        animator.SetInteger(animStraight, number);
        // 舉手，計算攻擊位置並顯示提醒
        if (number == 1)
            CalculateAttackPosition();
        else if (number == 2)
            AttackStraight();
        else if (number == 0)
            StopAttack();
    }

    private void CalculateAttackPosition()
    {
        Vector2 pos = Player.Singleton.transform.position;
        // 攻擊玩家前方2格位置
        attackGrid1 = pos + Player.Singleton.movement.MoveDirection * 2.0f;
        attackGrid1.x = Mathf.Floor(attackGrid1.x) + 0.5f;
        attackGrid1.y = Mathf.Floor(attackGrid1.y) + 0.5f;
        Debug.DrawLine(transform.position, attackGrid1, Color.red, 5);
        hint1.transform.position = attackGrid1;
        ObjectTempoControl.Singleton.AddToBeatAction(ShowAttackHint, TempoActionType.Half);
    }

    /// <summary>
    /// 顯示攻擊範圍
    /// </summary>
    private void ShowAttackHint()
    {
        hint1.color = new Color(1, 1, 1, hint1.color.a == 0 ? 0.5f : 0);
    }

    private void AttackStraight()
    {
        float distance = Vector2.Distance(attackGrid1, transform.position);
        // 伸長距離 = [(1 / 父物件縮放 / 圖片大小) = 1個單位] * 攻擊距離
        transform.localScale = new Vector3(1.0f / transform.parent.localScale.x / 86.7f * distance, 1, 1);
        // 旋轉
        transform.Rotate(Vector3.forward, Vector2.SignedAngle(Vector2.left, (attackGrid1 - (Vector2)transform.position).normalized));
    }

    private void StopAttack()
    {
        ObjectTempoControl.Singleton.RemoveToBeatAction(ShowAttackHint, TempoActionType.Half);
        hint1.color = new Color(1, 1, 1, 0);
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// 判斷有沒有打到東西
    /// </summary>
    public void DetectColliding()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(attackGrid1, Vector2.one * 0.95f, 0, Vector2.zero, 1, hitableLayer);
        if (hits != null)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.CompareTag("Player"))
                {
                    Player.Singleton.movement.Knock(Vector2.left, knockDistance, knockSpeed);
                    Player.Singleton.lifeSystem.Hurt(damage);
                }
                else if (hits[i].collider.CompareTag("Bomb"))
                    hits[i].collider.GetComponent<Bomb>().Knock(Vector2.left, knockSpeed, 1);
            }
        }
    }

    public void RaiseForSetor(int number)
    {
        Debug.Log(number);
        animator.SetInteger(animSector, number);
        // 舉手
        if (number == 1)
        {
            transform.localScale = new Vector3(0.5f, 1, 1);
            ObjectTempoControl.Singleton.AddToBeatAction(ShowSectorHint, TempoActionType.Half);
        }
        else if (number == 0)
        {
            ObjectTempoControl.Singleton.RemoveToBeatAction(ShowSectorHint, TempoActionType.Half);
            transform.localScale = Vector3.one;
            hintSector.color = new Color(1, 1, 1, 0);
            transform.rotation = Quaternion.identity;
        }
    }

    private void ShowSectorHint()
    {
        hintSector.color = new Color(1, 1, 1, hintSector.color.a == 0 ? 0.5f : 0);
    }

    public void DetectSectorCollision()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(new Vector2(5.5f, 0), new Vector2(21, 17), 0, Vector2.zero, 1, hitableLayer);
        if (hits != null)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.CompareTag("Player"))
                {
                    Player.Singleton.movement.Knock(Vector2.down, knockDistance * 2.0f, knockSpeed * 2.0f);
                    Player.Singleton.lifeSystem.Hurt(damage + 1);
                }
                else if (hits[i].collider.CompareTag("Bomb"))
                    hits[i].collider.GetComponent<Bomb>().Knock(Vector2.left, knockSpeed * 3.0f, 1);
            }
        }
    }

    private void RemoveTempoEvent()
    {
        ObjectTempoControl.Singleton.RemoveToBeatAction(ShowSectorHint, TempoActionType.Half);
        ObjectTempoControl.Singleton.RemoveToBeatAction(ShowAttackHint, TempoActionType.Half);
        gameObject.SetActive(false);
    }
}
