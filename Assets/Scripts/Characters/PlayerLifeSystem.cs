using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLifeSystem : MonoBehaviour
{
    [SerializeField]
    private int hp;
    public int Hp { get; private set; }

    public Image redEffectImage;

    // Start is called before the first frame update
    void Start()
    {
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        playerMovement.OnMiss += LossLife;
    }

    private void LossLife()
    {
        BreakHeart();
        StartCoroutine(ShowEffect());
    }

    private void BreakHeart()
    {

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
