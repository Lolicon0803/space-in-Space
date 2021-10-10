using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossSceneManager : MonoBehaviour
{
    public BigSquid boss;
    public string[] storyName;
    public TextWriter textWriter;

    public GameObject kiriBoss;
    public GameObject kiriperson1;
    public GameObject kiriperson2;
    public GameObject ship;

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
        action = new UnityAction(() => { });
        // 播過劇情表示第二次進來
        if (DataBase.Singleton.datas.readStories.ContainsKey(storyName[0]) && DataBase.Singleton.datas.readStories[storyName[0]])
        {
            StartCoroutine(WaitStartBoss());
        }
        //劇情
        else
        {
            //StartCoroutine(AdjustHittingUI());
            StartCoroutine(StartStory());
        }
        StartCoroutine(WinStory1());
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
        Player.Singleton.transform.position = new Vector3(-17.5f, -5.5f, 0);
        Camera.main.transform.position = new Vector3(-9f, -3f, -10);
        yield return new WaitForSeconds(0.5f);
        Vector3 destination = new Vector3(-14.5f, -5.5f, 0);
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
        float size = 10 * (16.0f / 9.0f) / ((float)Screen.width / Screen.height);
        while (Camera.main.orthographicSize < size)
        {
            Camera.main.orthographicSize += Time.deltaTime;
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, new Vector3(0, -2, -10), 2.8f * Time.deltaTime);
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
        float size = 10 * (16.0f / 9.0f) / ((float)Screen.width / Screen.height);
        Camera.main.orthographicSize = size;
        Camera.main.GetComponent<CameraAspect>().UpdateAspect();
        Camera.main.transform.position = new Vector3(0, -2, -10);
        Player.Singleton.transform.position = new Vector3(-14.5f, -5.5f, 0);
        Player.Singleton.movement.StopMove(false);
        textWriter.onEndStory.RemoveListener(action);
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
        //StopAllCoroutines();
        boss.SetActive();
        Player.Singleton.movement.canInput = true;
    }

    public IEnumerator WinStory1()
    {
        // 等待boss死亡
        while (boss.NowHP != -1)
            yield return null;
        Player.Singleton.movement.StopMove(false);
        Player.Singleton.movement.notMoveYet = true;
        Player.Singleton.transform.rotation = Quaternion.identity;
        textWriter.onEndStory.RemoveListener(action);
        action = new UnityAction(() => StartCoroutine(WinStory2()));
        textWriter.onEndStory.AddListener(action);
        textWriter.Init();
        textWriter.LoadStory(storyName[3] + ".txt");
        textWriter.NextStory();
    }

    public IEnumerator WinStory2()
    {
        Player.Singleton.movement.StopMove(false);
        Player.Singleton.movement.notMoveYet = true;
        yield return StartCoroutine(boss.ShowDieEffect());
        yield return new WaitForSeconds(1.0f);
        Player player = Player.Singleton;
        Vector2 destination = new Vector2(-11.5f, -5.5f);
        player.animationManager.PlayWalkAnimation();
        player.animationManager.bagJet.Play();
        while (Mathf.Abs(player.transform.position.x - destination.x) > Time.deltaTime * 3)
        {
            if (player.transform.position.x < destination.x)
            {
                player.transform.position += (Vector3)Vector2.right * Time.deltaTime * 3;
                player.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                player.transform.position += (Vector3)Vector2.left * Time.deltaTime * 3;
                player.transform.localScale = new Vector3(-1, 1, 1);
            }
            yield return null;
        }
        player.transform.position = new Vector3(destination.x, player.transform.position.y);
        player.animationManager.bagJet.Play();
        while (Mathf.Abs(player.transform.position.y - destination.y) > Time.deltaTime * 3)
        {
            if (player.transform.position.y < destination.y)
                player.transform.position += (Vector3)Vector2.up * Time.deltaTime * 3;
            else
                player.transform.position += (Vector3)Vector2.down * Time.deltaTime * 3;
            yield return null;
        }
        player.transform.position = destination;
        player.transform.localScale = new Vector3(-1, 1, 1);
        Player.Singleton.animationManager.BackWalkToIdle();
        yield return new WaitForSeconds(1.5f);
        destination = new Vector2(-14.5f, -5.2f);
        while (Vector2.Distance(kiriBoss.transform.position, destination) > Time.deltaTime * 3)
        {
            kiriBoss.transform.position += (Vector3)Vector2.right * 3 * Time.deltaTime;
            kiriperson1.transform.position += (Vector3)Vector2.right * 3 * Time.deltaTime;
            kiriperson2.transform.position += (Vector3)Vector2.right * 3 * Time.deltaTime;
            yield return null;
        }
        textWriter.onEndStory.RemoveListener(action);
        action = new UnityAction(() => StartCoroutine(WinStory3()));
        textWriter.onEndStory.AddListener(action);
        textWriter.Init();
        textWriter.LoadStory(storyName[4] + ".txt");
        textWriter.NextStory();
    }

    public IEnumerator WinStory3()
    {
        Player player = Player.Singleton;
        player.movement.StopMove(false);
        player.movement.notMoveYet = true;
        player.gameObject.layer = LayerMask.GetMask("Ignore Raycast");
        // 飛船進來
        Vector2 destination = new Vector2(-15.44f, -4.51f);
        while (Vector2.Distance(ship.transform.position, destination) > Time.deltaTime * 3)
        {
            ship.transform.position += (Vector3)Vector2.right * 3 * Time.deltaTime;
            yield return null;
        }
        // 玩家往上
        destination = new Vector2(-11.5f, -3.5f);
        player.animationManager.bagJet.Play();
        while (Mathf.Abs(player.transform.position.y - destination.y) > Time.deltaTime * 3)
        {
            player.transform.position += (Vector3)Vector2.up * Time.deltaTime * 3;
            yield return null;
        }
        player.transform.position = destination;
        destination.x = -14.5f;
        player.animationManager.bagJet.Play();
        // 玩家往左
        while (Mathf.Abs(player.transform.position.x - destination.x) > Time.deltaTime * 3)
        {
            player.transform.position += (Vector3)Vector2.left * Time.deltaTime * 3;
            yield return null;
        }
        player.transform.position = destination;
        destination.y = -4.25f;
        // 玩家往下
        while (Mathf.Abs(player.transform.position.x - destination.x) > Time.deltaTime)
        {
            player.transform.position += (Vector3)Vector2.down * Time.deltaTime;
            yield return null;
        }
        player.transform.localScale = new Vector3(1, 1, 1);
        player.transform.position = destination;
        player.transform.parent = ship.transform;
        destination.x = -10.5f;
        destination.y = -3.5f;
        // 飛船慢慢向前
        while (Vector2.Distance(ship.transform.position, destination) > Time.deltaTime)
        {
            ship.transform.position = Vector2.MoveTowards(ship.transform.position, destination, Time.deltaTime);
            yield return null;
        }
        ship.transform.position = destination;
        destination.x = 23.5f;
        destination.y = 12.5f;
        // 飛船衝出去然後切人員名單
        while (Vector2.Distance(ship.transform.position, destination) > Time.deltaTime * 20)
        {
            ship.transform.position = Vector2.MoveTowards(ship.transform.position, destination, 20 * Time.deltaTime);
            yield return null;
        }
        Destroy(player.lifeSystem.lifeCanvas.gameObject);
        Destroy(player.lifeSystem.dieCanvas.gameObject);
        Destroy(player.gameObject);
        SceneController.Singleton.LoadSceneAsync(9, true);
    }
}
