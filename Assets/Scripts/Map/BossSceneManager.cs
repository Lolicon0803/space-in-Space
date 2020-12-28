using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossSceneManager : MonoBehaviour
{
    public BigSquid boss;
    public string[] storyName;
    public TextWriter textWriter;

    private UnityAction action;

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
        if (DataBase.Singleton.datas.readStories.ContainsKey(storyName[0]) && DataBase.Singleton.datas.readStories[storyName[0]])
        {
            StartCoroutine(WaitStartBoss());
        }
        // 劇情
        else
        {
            StartCoroutine(AdjustHittingUI());
            StartCoroutine(StartStory());
        }
    }

    private IEnumerator AdjustHittingUI()
    {
        HittingController hc = FindObjectOfType<HittingController>();
        Vector2 pos;
        while (true)
        {
            pos = Camera.main.transform.position;
            pos.y -= Camera.main.orthographicSize;
            pos.y += 1;
            hc.transform.position = pos;
            yield return null;
        }
    }

    private IEnumerator StartStory()
    {
        Player.Singleton.movement.StopMove(false);
        Player.Singleton.movement.notMoveYet = true;
        Player.Singleton.transform.rotation = Quaternion.identity;
        Player.Singleton.transform.position = new Vector3(-17.5f, -3.5f, 0);
        yield return new WaitForSeconds(0.5f);
        Vector3 destination = new Vector3(-14.5f, -3.5f, 0);
        Player.Singleton.animationManager.PlayWalkAnimation();
        yield return null;
        while (Mathf.Abs(Player.Singleton.transform.position.x - destination.x) > Time.deltaTime * 2)
        {
            Player.Singleton.transform.position += (Vector3)Vector2.right * Time.deltaTime * 2;
            yield return null;
        }
        Player.Singleton.animationManager.BackWalkToIdle();
        Player.Singleton.transform.position = destination;

        DataBase.Singleton.datas.readStories[storyName[0]] = true;
        action = new UnityAction(() => StartCoroutine(ZoomOutCamera()));
        textWriter.onEndStory.AddListener(action);
        textWriter.Init();
        textWriter.LoadStory(storyName[0] + ".txt");
        textWriter.NextStory();
    }

    private IEnumerator ZoomOutCamera()
    {
        Player.Singleton.movement.StopMove(false);
        Player.Singleton.movement.notMoveYet = true;
        while (Camera.main.orthographicSize < 10)
        {
            Camera.main.orthographicSize += Time.deltaTime;
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, new Vector3(0, 0, -10), 2.8f * Time.deltaTime);
            yield return null;
        }
        textWriter.onEndStory.RemoveListener(action);
        action = new UnityAction(() => StartCoroutine(StartStory2()));
        textWriter.onEndStory.AddListener(action);
        textWriter.Init();
        textWriter.LoadStory(storyName[1] + ".txt");
        textWriter.NextStory();
    }

    private IEnumerator StartStory2()
    {
        Player.Singleton.movement.StopMove(false);
        Player.Singleton.movement.notMoveYet = true;
        boss.hand.GetComponent<Animator>().SetInteger("Sector", 1);
        yield return new WaitForSeconds(1.0f);
        boss.hand.GetComponent<Animator>().SetInteger("Sector", 2);
        Vector2 destination = Player.Singleton.transform.position + Vector3.left;
        while (Mathf.Abs(Player.Singleton.transform.position.x - destination.x) > Time.deltaTime * 3)
        {
            Player.Singleton.transform.position += (Vector3)Vector2.left * Time.deltaTime * 3;
            yield return null;
        }
        Player.Singleton.transform.position = destination;
        yield return new WaitForSeconds(1.0f);
        boss.hand.GetComponent<Animator>().SetInteger("Sector", 0);
        Player.Singleton.movement.ZeroMoveDirection();
        yield return new WaitForSeconds(0.5f);
        textWriter.onEndStory.RemoveListener(action);
        action = new UnityAction(() => StartBoss());
        textWriter.onEndStory.AddListener(action);
        textWriter.Init();
        textWriter.LoadStory(storyName[2] + ".txt");
        textWriter.NextStory();
        yield return null;
    }

    private IEnumerator WaitStartBoss()
    {
        Player.Singleton.movement.StopMove(false);
        Player.Singleton.movement.notMoveYet = true;
        Camera.main.GetComponent<CameraAspect>().originSize = 10;
        Camera.main.orthographicSize = 10;
        Camera.main.GetComponent<CameraAspect>().UpdateAspect();
        Camera.main.transform.position = new Vector3(0, 0, -10);
        Player.Singleton.transform.position = new Vector3(-14.5f, -3.5f, 0);
        Player.Singleton.movement.StopMove(false);
        FindObjectOfType<HittingController>().transform.localPosition = new Vector3(0, -6, 10);
        action = new UnityAction(() => StartBoss());
        textWriter.onEndStory.AddListener(action);
        yield return new WaitForSeconds(0.5f);
        textWriter.Init();
        textWriter.LoadStory(storyName[2] + ".txt");
        textWriter.NextStory();
    }

    public void StartBoss()
    {
        Player.Singleton.movement.StopMove(false);
        Player.Singleton.movement.notMoveYet = true;
        StopAllCoroutines();
        boss.SetActive();
        Player.Singleton.movement.canInput = true;
    }
}
