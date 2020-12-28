using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class SceneController : MonoBehaviour
{
    AsyncOperation operation;

    public bool UseFadeInout { get; private set; }
    public Image fadeInOutImage;

    private static SceneController singleton;
    public static SceneController Singleton
    {
        get
        {
            if (singleton == null)
            {
                singleton = FindObjectOfType(typeof(SceneController)) as SceneController;
            }
            return singleton;
        }

    }

    public UnityAction OnFadeInStart;
    public UnityAction OnFadeInEnd;
    public UnityAction OnFadeOutStart;
    public UnityAction OnFadeOutEnd;

    public List<int> storyScene = new List<int>() { 2, 3 };

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        // 退出遊戲時如果在劇情場景則存檔
        if (storyScene.FindIndex(index => index == SceneManager.GetActiveScene().buildIndex) >= 0)
        {
            Debug.Log("退出並存檔");
            DataBase.Singleton.Save();
        }
    }

    static public void GoToScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void BackToMainMenu()
    {
        // 回主選單時若在故事場景則存檔
        if (storyScene.FindIndex(index => index == SceneManager.GetActiveScene().buildIndex) >= 0)
        {
            Debug.Log("返回主選單並存檔");
            DataBase.Singleton.Save();
        }
        LoadSceneAsync(0, false);
    }

    /// <summary>
    /// 切換至編號為index的場景
    /// </summary>
    /// <param name="index">場景編號</param>
    /// <param name="useFadeInOut">是否使用淡入淡出效果</param>
    public void LoadSceneAsync(int index, bool useFadeInOut)
    {
        UseFadeInout = useFadeInOut;
        StartCoroutine(LoadSceneAsync(index));
    }

    private void Update()
    {

    }

    private IEnumerator LoadSceneAsync(int index)
    {
        operation = SceneManager.LoadSceneAsync(index);
        operation.allowSceneActivation = false;

        if (UseFadeInout)
            yield return StartCoroutine(ShowFadeIn());

        operation.allowSceneActivation = true;
        yield return new WaitUntil(() => operation.isDone);

        if (UseFadeInout)
            yield return StartCoroutine(ShowFadeOut());
    }

    public void FadeIn()
    {
        StartCoroutine(ShowFadeIn());
    }

    public void FadeOut()
    {
        StartCoroutine(ShowFadeOut());
    }

    public IEnumerator ShowFadeIn()
    {
        fadeInOutImage.color = new Color(1, 1, 1, 0);
        OnFadeInStart?.Invoke();
        while (fadeInOutImage.color.a < 1)
        {
            Debug.Log(fadeInOutImage.color);
            fadeInOutImage.color = new Color(1, 1, 1, fadeInOutImage.color.a + Time.deltaTime * 1.0f);
            yield return null;
        }
        OnFadeInEnd?.Invoke();
        fadeInOutImage.color = new Color(1, 1, 1, 1);
    }

    public IEnumerator ShowFadeOut()
    {
        fadeInOutImage.color = new Color(1, 1, 1, 1);
        OnFadeOutStart?.Invoke();
        while (fadeInOutImage.color.a > 0)
        {
            fadeInOutImage.color = new Color(1, 1, 1, fadeInOutImage.color.a - Time.deltaTime * 1.0f);
            yield return null;
        }
        OnFadeOutEnd?.Invoke();
        fadeInOutImage.color = new Color(1, 1, 1, 0);
    }

}
