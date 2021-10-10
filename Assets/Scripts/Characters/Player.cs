using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private static Player singleton;
    public static Player Singleton
    {
        get
        {
            if (singleton == null)
            {
                //singleton = FindObjectOfType(typeof(Player)) as Player;
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
            if (SceneManager.GetActiveScene().buildIndex == 6)
            {
                if (lifeSystem.lifeCanvas != null)
                    DontDestroyOnLoad(lifeSystem.lifeCanvas.gameObject);
                if (lifeSystem.dieCanvas != null)
                    DontDestroyOnLoad(lifeSystem.dieCanvas.gameObject);
                DontDestroyOnLoad(gameObject);
            }
        }
        else if (singleton != this)
        {
            //Debug.Log("在這裡嗎");
            //if (lifeSystem.lifeCanvas != null)
            //    Destroy(lifeSystem.lifeCanvas.gameObject);
            //if (lifeSystem.dieCanvas != null)
            //    Destroy(lifeSystem.dieCanvas.gameObject);
            //Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.I) && Input.GetKey(KeyCode.N))
        {
            Time.timeScale = 10;
        }
        else
            Time.timeScale = 1;
    }

    public void DestroyPlayer()
    {
        if (lifeSystem.lifeCanvas != null)
            SceneManager.MoveGameObjectToScene(lifeSystem.lifeCanvas.gameObject, SceneManager.GetActiveScene());
        if (lifeSystem.dieCanvas != null)
            SceneManager.MoveGameObjectToScene(lifeSystem.dieCanvas.gameObject, SceneManager.GetActiveScene());
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        SceneManager.sceneLoaded -= movement.RegisterTempo;
    }
}
