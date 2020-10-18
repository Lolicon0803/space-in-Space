using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Characters;

public class WhiteHole : MonoBehaviour
{
    // 範圍
    public float radius;
    // 推人推多遠
    public float pushUnit;
    // 推人時的速度
    public float pushSpeed;

    private void Start()
    {
        GetComponent<BoxCollider2D>().size = new Vector2(radius, radius);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            Vector2 position = collision.transform.position;
            Debug.Log(position);
            // 從左邊撞
            if (position.x >= transform.position.x + radius / 2.0f * Constants.moveUnit)
                player.Knock(Vector2.right, pushUnit, pushSpeed);
            else if (position.x <= transform.position.x - radius / 2.0f * Constants.moveUnit)
                player.Knock(Vector2.left, pushUnit, pushSpeed);
            else if (position.y >= transform.position.y + radius / 2.0f * Constants.moveUnit)
                player.Knock(Vector2.up, pushUnit, pushSpeed);
            else if (position.y <= transform.position.y - radius / 2.0f * Constants.moveUnit)
                player.Knock(Vector2.down, pushUnit, pushSpeed);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, radius * Vector2.one);
    }

}
