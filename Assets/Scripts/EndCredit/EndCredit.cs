using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class EndCredit : MonoBehaviour
{
    public GameObject rollObj;
    public GameObject endNode;
    public float rollSpeed;
    private bool isRolling;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("StartRoll", 3f);
    }
    void StartRoll()
    {
        isRolling = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (isRolling)
        {
            rollObj.transform.localPosition =
                rollObj.transform.localPosition + new Vector3(0, rollSpeed * Time.deltaTime, 0);
            if (rollObj.transform.localPosition.y >= -endNode.transform.localPosition.y)
            {
                rollObj.transform.localPosition = new Vector3(0, -endNode.transform.localPosition.y, 0);
                isRolling = false;
                Invoke("OnEnd", 10.0f);
            }
        }
    }
    void OnEnd()
    {
        SceneController.Singleton.LoadSceneAsync(0, true)
    }
}
