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
    private int nowHp;

    // 玩家死亡後的起始場景，最好不要用這個。沒設定則為現在場景
    // 不是指地圖設計上的場景，是指Asset/Scene裡的檔。
    //public Scene startScene;
    // 復活點。
    public Vector2 startPosition;

    public Canvas canvas;
    public Image redEffectImage;
    public Image heartImagePrefab;

    public Image blackImage;

    public Sprite fullHeart;
    public Sprite breakHeart;
    public Sprite BlackHear;

    private Image[] heartImages;

    // 最後一顆愛心的索引值
    private int lastHeartIndex;
    // 狀態，2: 滿，1: 裂，0: 黑。
    private HeartStatus lastHeartState;

    private PlayerMovement playerMovement;

    private bool isDie;

    // Start is called before the first frame update
    void Start()
    {
        isDie = false;
        playerMovement = GetComponent<PlayerMovement>();
        //playerMovement.OnMiss += LossLife;
        playerMovement.OnError += LossLife;
        InitializeHHeart();
    }

    private void InitializeHHeart()
    {
        nowHp = maxHp;
        int total = nowHp / 2 + nowHp % 2;
        if (heartImages == null)
        {
            heartImages = new Image[total];
            for (int i = 0; i < total; i++)
            {
                heartImages[i] = Instantiate(heartImagePrefab, canvas.transform);
                heartImages[i].rectTransform.offsetMin = new Vector2(heartImages[i].rectTransform.offsetMin.x + 75 * i, heartImages[i].rectTransform.offsetMin.y);
                heartImages[i].rectTransform.offsetMax = new Vector2(heartImages[i].rectTransform.offsetMax.x + 75 * i, heartImages[i].rectTransform.offsetMax.y);
            }
        }
        foreach (Image image in heartImages)
            image.sprite = fullHeart;

        lastHeartIndex = total - 1;
        if (nowHp % 2 != 0)
        {
            heartImages[lastHeartIndex].sprite = breakHeart;
            lastHeartState = HeartStatus.Break;
        }
        else
            lastHeartState = HeartStatus.Full;
    }

    /// <summary>
    /// 設定重生點。
    /// </summary>
    /// <param name="position">重生點。</param>
    public void SetStartPosition(Vector2 position)
    {
        startPosition = position;
    }

    /// <summary>
    /// When miss or error, loss one hp and show red screen.
    /// </summary>
    public void LossLife()
    {

        //if (!Player.Singleton.movement.firstTimeMiss && !isDie)
        //{
        //playerMovement.canInput = false;
        BreakHeart();
        StartCoroutine(ShowRedEffect());
        //}
    }

    /// <summary>
    /// When miss or error, loss one hp and show red screen.
    /// </summary>
    public void Hurt()
    {
        if (!isDie)
        {
            BreakHeart();
            StartCoroutine(ShowRedEffect());
        }
    }

    /// <summary>
    /// HP minus1 and check gameover.
    /// </summary>
    private void BreakHeart()
    {
        nowHp--;
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
        // Gameover.
        if (nowHp <= 0)
            GameOver();
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
        isDie = true;
        transform.localScale = Vector3.zero;
        playerMovement.Die();
        StartCoroutine(ShowBlackEffect());
    }

    /// <summary>
    /// 黑屏。
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowBlackEffect()
    {
        // 可能要改。愛心動態生成所以圖層比黑屏高。
        foreach (Image image in heartImages)
            image.color = new Color(1, 1, 1, 0);

        // 黑屏慢慢出現。
        float alpha = 0;
        blackImage.color = new Color(0, 0, 0, alpha);
        while (alpha < 1)
        {
            alpha += 0.5f * Time.deltaTime;
            redEffectImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        InitializeHHeart();

        // 玩家回到起始點。
        playerMovement.movePoint = startPosition;
        transform.position = startPosition;
        // 黑屏結束
        while (alpha > 0)
        {
            alpha -= 0.5f * Time.deltaTime;
            redEffectImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        // 愛心出來。
        foreach (Image image in heartImages)
            image.color = new Color(1, 1, 1, 1);
        playerMovement.ResetStatus();
        isDie = false;
    }
}
