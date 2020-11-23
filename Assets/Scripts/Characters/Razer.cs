using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Characters;



public class Razer : MonoBehaviour
{
    public int damage = 0;
    private SpriteRenderer sprite;
    private BoxCollider2D collider2D;

    private AudioSource audioSource;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        collider2D = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    public void SetSize(float x, float y)
    {
        sprite.size = new Vector2(x, y);
        collider2D.size = new Vector2(sprite.size.x, sprite.size.y - 0.01f);
        collider2D.offset = new Vector2(sprite.size.x / 2.0f, 0);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && sprite.size.x > 0)
        {
            Debug.Log("雷射射到。");
            Player.Singleton.lifeSystem.Hurt(damage);
        }
    }
}
