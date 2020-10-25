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
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
        }
        //movement = GetComponent<PlayerMovement>();
        //lifeSystem = GetComponent<PlayerLifeSystem>();
        //audioManager = GetComponent<PlayerAudioManager>();
        //animationManager = GetComponent<PlayerAnimationManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
