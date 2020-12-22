using System.Collections;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    private float movementSpeed;
    private HittingController hittingController;
    private float BPS;
    private float moveTime;

    private Vector3 startPos;
    private Vector3 endPos;
    private NoteController noteController;
    private IEnumerator coroutine;

    public void hit()
    {
        StopCoroutine(coroutine);
        StartCoroutine(Disappear());
    }

    void Start()
    {
        hittingController = GameObject.Find("HittingUI").GetComponent<HittingController>();
        noteController = GameObject.Find("NoteHolder").GetComponent<NoteController>();
        BPS = (float)(hittingController.audioEngine.BPM / 60);
        moveTime = Mathf.Abs(transform.localPosition.x) / BPS;

        startPos = transform.localPosition;
        if (transform.localPosition.x < 0)
        {
            endPos = new Vector3(0.1f, transform.localPosition.y, transform.localPosition.z);
        }
        else
        {
            endPos = new Vector3(-0.1f, transform.localPosition.y, transform.localPosition.z);
        }

        coroutine = Move();
        StartCoroutine(coroutine);
    }

    IEnumerator Move()
    {
        float timeElapsed = 0;

        while (timeElapsed < moveTime)
        {
            transform.localPosition = Vector3.Lerp(startPos, endPos, timeElapsed / moveTime);

            disappearIfArrived();

            timeElapsed += Time.deltaTime;

            yield return null;
            if (hittingController.isStop())
            {
                Destroy(gameObject);
            }
        }
        noteController.setLastDelay((float)(hittingController.audioEngine.getTime() % (1000 / BPS)));
        Destroy(gameObject);
    }

    IEnumerator Disappear()
    {
        float time = 0;

        while (time < .5f)
        {
            time += Time.deltaTime;
            GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1 - time);
            yield return null;
        }

        Destroy(gameObject);
    }

    void disappearIfArrived()
    {
        if (endPos.x > 0 && transform.localPosition.x > 0 || endPos.x < 0 && transform.localPosition.x < 0)
        {
            gameObject.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0);
        }
    }
}
