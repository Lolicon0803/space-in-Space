using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public Canvas canvas;
    public Image redEffectImage;
    public Image heartImagePrefab;

    public Sprite fullHeart;
    public Sprite breakHeart;
    public Sprite BlackHear;

    private Image[] heartImages;

    // 最後一顆愛心的索引值
    private int lastHeartIndex;
    // 狀態，2: 滿，1: 裂，0: 黑。
    private HeartStatus lastHeartState;

    // Start is called before the first frame update
    void Start()
    {
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        playerMovement.OnMiss += LossLife;

        nowHp = maxHp;

        int total = nowHp / 2 + nowHp % 2;
        heartImages = new Image[total];
        lastHeartIndex = total - 1;
        for (int i = 0; i < total; i++)
        {
            heartImages[i] = Instantiate(heartImagePrefab, canvas.transform);
            heartImages[i].rectTransform.offsetMin = new Vector2(heartImages[i].rectTransform.offsetMin.x + 75 * i, heartImages[i].rectTransform.offsetMin.y);
            heartImages[i].rectTransform.offsetMax = new Vector2(heartImages[i].rectTransform.offsetMax.x + 75 * i, heartImages[i].rectTransform.offsetMax.y);
        }
        if (nowHp % 2 != 0)
        {
            heartImages[lastHeartIndex].sprite = breakHeart;
            lastHeartState = HeartStatus.Break;
        }
        else
            lastHeartState = HeartStatus.Full;
    }

    private void LossLife()
    {
        BreakHeart();
        StartCoroutine(ShowEffect());
    }

    private void BreakHeart()
    {
        nowHp--;
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

        if (nowHp == 0)
        {
            //GameOver
            nowHp = maxHp;
            int total = nowHp / 2 + nowHp % 2;
            lastHeartIndex = total - 1;
        }
    }

    private IEnumerator ShowEffect()
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
}
