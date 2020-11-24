using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{


    static public void goToScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    static public void backToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
