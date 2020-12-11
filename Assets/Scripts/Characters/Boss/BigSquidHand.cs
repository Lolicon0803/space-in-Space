using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigSquidHand : MonoBehaviour
{
    private int damage = 1;
    private float knockSpeed = 25;
    private float knockDistance = 5;

    private bool isAttacking = false;
    private Vector2 knockDirection;

    public void SetKnockDirection(Vector2 direction)
    {
        knockDirection = direction;
    }

    public void StartAttack()
    {
        isAttacking = true;
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isAttacking)
                Player.Singleton.movement.Knock(knockDirection, knockDistance, knockSpeed);
            else
                Player.Singleton.movement.Knock(Vector2.zero, 3, 20);
            Player.Singleton.lifeSystem.Hurt(1);
        }
        else if (collision.CompareTag("Bomb") && isAttacking)
        {
            collision.GetComponent<Bomb>().Knock(knockDirection, knockSpeed, 1);
        }
    }
}
