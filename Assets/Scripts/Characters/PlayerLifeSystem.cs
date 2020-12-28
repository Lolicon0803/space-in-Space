using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum HeartStatus
{
    Full,
    Break,
    Black
}

public class PlayerLifeSystem : MonoBehaviour
{
    public int maxHp;
    public int NowHp { get; set; }

    // 受傷後無敵時間
    public int invincibleTempo;
    // 成功噴射幾次就回血
    public int recoverAfterShoot;

    // 復活場景
    private int startSceneIndex;
    // 復活點。
    private Vector2 startPosition;

    public Canvas lifeCanvas;
    public CanvasGroup dieCanvas;
    // 受傷紅屏
    public Image redEffectImage;
    // 愛心
    public Image heartImagePrefab;
    // 死亡黑屏
    public Image blackCircleImage;
    public Image blackBgImage;

    public Sprite fullHeart;
    public Sprite breakHeart;
    public Sprite BlackHear;
    public Animator recoverHeart;

    private Image[] heartImages;

    // 最後一顆愛心的索引值
    private int lastHeartIndex;
    // 狀態，2: 滿，1: 裂，0: 黑。
    private HeartStatus lastHeartState;

    private PlayerMovement playerMovement;

    public bool IsInvincible { get; set; }
    private int invincibleCount;
    private int recoverCount;
    public bool IsDie { get; private set; }

    public delegate void PlayerStatus();
    public event PlayerStatus OnDie;

    // Start is called before the first frame update
    void Start()
    {
        blackBgImage.rectTransform.offsetMax = Vector2.zero;
        blackBgImage.rectTransform.offsetMin = Vector2.zero;
        startSceneIndex = -1;
        startPosition = new Vector2(-16.5f, -8.5f);
        Vector3 pos = transform.position;
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f;
        transform.position = pos;
        IsDie = false;
        IsInvincible = false;
        // IsInvincible = true;
        invincibleCount = 0;
        recoverCount = 0;
        playerMovement = GetComponent<PlayerMovement>();
        playerMovement.OnFireBag += Recover;
        playerMovement.OnError += LossLife;
        NowHp = maxHp;
        InitializeHHeart();
    }

    public void InitializeHHeart()
    {
        if (dieCanvas == null || lifeCanvas == null)
            return;
        int total = NowHp / 2 + NowHp % 2;
        if (heartImages == null)
        {
            heartImages = new Image[total];
            for (int i = 0; i < total; i++)
            {
                heartImages[i] = Instantiate(heartImagePrefab, lifeCanvas.transform);
                heartImages[i].rectTransform.offsetMin = new Vector2(heartImages[i].rectTransform.offsetMin.x + 75 * i, heartImages[i].rectTransform.offsetMin.y);
                heartImages[i].rectTransform.offsetMax = new Vector2(heartImages[i].rectTransform.offsetMax.x + 75 * i, heartImages[i].rectTransform.offsetMax.y);
            }
        }
        foreach (Image image in heartImages)
            image.sprite = BlackHear;
        lastHeartIndex = -1;
        for (int i = 0; i < NowHp - 1; i += 2)
        {
            lastHeartIndex++;
            heartImages[lastHeartIndex].sprite = fullHeart;
        }
        lastHeartState = HeartStatus.Full;
        if (NowHp % 2 != 0)
        {
            lastHeartIndex++;
            heartImages[lastHeartIndex].sprite = breakHeart;
            lastHeartState = HeartStatus.Break;
        }
    }

    /// <summary>
    /// 設定重生點。
    /// </summary>
    /// <param name="position">重生點。</param>
    public void SetStartPosition(int index, Vector2 position)
    {
        startPosition = position;
        if (index == -1)
            startSceneIndex = SceneManager.GetActiveScene().buildIndex;
        else
            startSceneIndex = index;
    }

    /// <summary>
    /// When miss or error, loss one hp and show red screen.
    /// </summary>
    public void LossLife()
    {
        if (!IsDie)
        {
            //isInvincible = true;
            //ObjectTempoControl.Singleton.AddToBeatAction(RemoveInvincibleStatus, TempoActionType.Whole);
            BreakHeart();
            StartCoroutine(ShowRedEffect());
        }
    }

    /// <summary>
    /// When miss or error, loss one hp and show red screen.
    /// </summary>
    public void Hurt(int number = 1)
    {
        if (!IsDie && !IsInvincible)
        {
            IsInvincible = true;
            ObjectTempoControl.Singleton.AddToBeatAction(RemoveInvincibleStatus, TempoActionType.Whole);
            for (int i = 0; i < number && !IsDie; i++)
                BreakHeart();
            StartCoroutine(ShowRedEffect());
        }
    }

    private void RemoveInvincibleStatus()
    {
        invincibleCount++;
        if (invincibleCount == invincibleTempo)
        {
            IsInvincible = false;
            invincibleCount = 0;
            ObjectTempoControl.Singleton.RemoveToBeatAction(RemoveInvincibleStatus, TempoActionType.Whole);
        }
    }

    /// <summary>
    /// HP minus1 and check gameover.
    /// </summary>
    private void BreakHeart()
    {
        NowHp--;
        recoverCount = 0;
        if (lastHeartIndex >= 0)
        {
            // 換圖片用
            switch (lastHeartState)
            {
                case HeartStatus.Full:
                    lastHeartState = HeartStatus.Break;
                    heartImages[lastHeartIndex].sprite = breakHeart;
                    break;
                case HeartStatus.Break:
                    lastHeartState = HeartStatus.Full;
                    heartImages[lastHeartIndex].sprite = BlackHear;
                    lastHeartIndex--;
                    break;
                default:
                    break;
            }
        }
        // Gameover.
        if (NowHp <= 0)
            GameOver();
    }

    private void Recover(Vector2 direction)
    {
        if (NowHp != maxHp)
        {
            recoverCount++;
            if (recoverCount == recoverAfterShoot)
            {
                recoverHeart.Play("Move");

                recoverCount = 0;
                NowHp++;
                // 換圖片用
                switch (lastHeartState)
                {
                    case HeartStatus.Full:
                        lastHeartState = HeartStatus.Break;
                        heartImages[lastHeartIndex + 1].sprite = breakHeart;
                        lastHeartIndex++;
                        break;
                    case HeartStatus.Break:
                        lastHeartState = HeartStatus.Full;
                        heartImages[lastHeartIndex].sprite = fullHeart;
                        break;
                    case HeartStatus.Black:
                        lastHeartState = HeartStatus.Break;
                        heartImages[lastHeartIndex].sprite = breakHeart;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 紅屏。
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowRedEffect()
    {
        redEffectImage.color = new Color(1, 0, 0, 0.25f);
        float alpha = 0.25f;
        while (alpha > 0)
        {
            alpha -= 0.5f * Time.deltaTime;
            redEffectImage.color = new Color(1, 0, 0, alpha);
            yield return null;
        }
    }

    /// <summary>
    /// Process gameover.
    /// </summary>
    public void GameOver()
    {
        playerMovement.Die();
        OnDie?.Invoke();
        IsDie = true;
        //transform.localScale = Vector3.zero;
        StartCoroutine(ShowBlackEffect());
    }

    /// <summary>
    /// 黑屏。
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowBlackEffect()
    {
        // 黑圈縮小聚焦到玩家上
        blackCircleImage.rectTransform.sizeDelta = new Vector2(4096, 4096);
        blackCircleImage.rectTransform.position = Camera.main.WorldToScreenPoint(transform.position);
        dieCanvas.alpha = 1;
        yield return ShowBlackCircle(new Vector2(256, 256));
        // 播玩家死亡動畫
        yield return new WaitForSeconds(1.0f);
        // 黑圈縮到底
        yield return ShowBlackCircle(Vector2.zero);

        NowHp = maxHp;
        InitializeHHeart();

        // 玩家回到起始點。
        playerMovement.movePoint = startPosition;
        transform.position = startPosition;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        // 同場景不轉
        // 等轉場景的程式碼完整再接過去
        if (startSceneIndex != -1 && startSceneIndex != SceneManager.GetActiveScene().buildIndex)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(startSceneIndex);
            while (!operation.isDone)
            {
                Debug.Log("Wait load scene");
                yield return null;
            }
        }
        yield return null;
        blackCircleImage.rectTransform.position = Camera.main.WorldToScreenPoint(transform.position);
        // 黑圈打開
        yield return ShowBlackCircle(new Vector2(256, 256));
        // 看有沒有復活動畫
        yield return new WaitForSeconds(1.0f);
        // 黑圈全開
        yield return ShowBlackCircle(new Vector2(4096, 4096));
        dieCanvas.alpha = 0;
        playerMovement.ResetStatus();
        IsDie = false;
    }

    private IEnumerator ShowBlackCircle(Vector2 destination)
    {
        while (Vector2.Distance(blackCircleImage.rectTransform.sizeDelta, destination) > Mathf.Epsilon)
        {
            blackCircleImage.rectTransform.sizeDelta = Vector2.MoveTowards(blackCircleImage.rectTransform.sizeDelta, destination, 4000 * Time.deltaTime);
            yield return null;
        }
        blackCircleImage.rectTransform.sizeDelta = destination;
    }
}
