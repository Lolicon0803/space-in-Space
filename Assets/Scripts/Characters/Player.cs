using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player singleton;
    public static Player Singleton
    {
        get
        {
            if (singleton == null)
            {
                singleton = FindObjectOfType(typeof(Player)) as Player;
            }
            return singleton;
        }

    }

    public PlayerMovement movement;
    public PlayerLifeSystem lifeSystem;
    public PlayerAudioManager audioManager;
    public PlayerAnimationManager animationManager;

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            if (lifeSystem.lifeCanvas != null)
                DontDestroyOnLoad(lifeSystem.lifeCanvas.gameObject);
            if (lifeSystem.dieCanvas != null)
                DontDestroyOnLoad(lifeSystem.dieCanvas.gameObject);
            DontDestroyOnLoad(gameObject);
        }
        else if (singleton != this)
        {
            if (lifeSystem.lifeCanvas != null)
                Destroy(lifeSystem.lifeCanvas.gameObject);
            if (lifeSystem.dieCanvas != null)
                Destroy(lifeSystem.dieCanvas.gameObject);
            Destroy(gameObject);
        }
    }

    public void DestroyPlayer()
    {
        if (lifeSystem.lifeCanvas != null)
            Destroy(lifeSystem.lifeCanvas.gameObject);
        if (lifeSystem.dieCanvas != null)
            Destroy(lifeSystem.dieCanvas.gameObject);
        Destroy(gameObject);
    }
}
