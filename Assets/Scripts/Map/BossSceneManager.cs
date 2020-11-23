using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSceneManager : MonoBehaviour
{
    public BigSquid boss;

    private static BossSceneManager singleton;
    public static BossSceneManager Singleton
    {
        get
        {
            if (singleton == null)
            {
                singleton = FindObjectOfType(typeof(BossSceneManager)) as BossSceneManager;
            }
            return singleton;
        }

    }

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
        }
    }

    public void StartBoss()
    {
        boss.SetActive();
        Player.Singleton.movement.canInput = true;
    }
}
