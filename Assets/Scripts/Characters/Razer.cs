using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Characters;



public class Razer : MonoBehaviour
{
    public int damage = 0;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("雷射射到。");
            Player.Singleton.lifeSystem.Hurt(damage);
        }
    }
}
