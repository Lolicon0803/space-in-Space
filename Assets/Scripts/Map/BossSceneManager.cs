using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSceneManager : MonoBehaviour
{
    public BigSquid boss;
    public Npc npc;

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

    private void Start()
    {
        // 播過劇情表示第二次進來
      //  if (DataBase.Singleton.readStories.ContainsKey(npc.storyName) && DataBase.Singleton.readStories[npc.storyName])
            StartBoss();
        // 劇情，強制移動玩家
      //  else
            StartCoroutine(ForceMovePlayer());
    }

    private IEnumerator ForceMovePlayer()
    {
        // 照理說不應該要這行
        while(Player.Singleton == null)
        {
            Debug.Log("沒有玩家");
            yield return null;
        }
        Player.Singleton.movement.StopMove();
        Player.Singleton.transform.rotation = Quaternion.identity;
        Player.Singleton.transform.position = new Vector3(-17.214f, -3.579f, 0);
        yield return new WaitForSeconds(0.5f);
        Vector3 destination = new Vector3(-16.141f, -3.579f, 0);
        Player.Singleton.animationManager.PlayWalkAnimation();
        yield return null;
        while (Mathf.Abs(Player.Singleton.transform.position.x - destination.x) > Time.deltaTime)
        {
            Player.Singleton.transform.position += (Vector3)Vector2.right * Time.deltaTime;
            yield return null;
        }
        Player.Singleton.animationManager.BackWalkToIdle();
        Player.Singleton.transform.position = destination;
    }

    public void StartBoss()
    {
        boss.SetActive();
        Player.Singleton.movement.canInput = true;
    }
}
