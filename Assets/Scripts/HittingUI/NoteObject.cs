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

    // Start is called before the first frame update
    void Start()
    {
        hittingController = GameObject.Find("HittingUI").GetComponent<HittingController>();
        noteController = GameObject.Find("NoteHolder").GetComponent<NoteController>();
        BPS = (float)(hittingController.audioEngine.BPM / 60);
        moveTime = Mathf.Abs(transform.position.x) / BPS;

        startPos = transform.position;
        if (transform.position.x < 0)
        {
            endPos = new Vector3(0.1f, transform.position.y, transform.position.z);
        }
        else
        {
            endPos = new Vector3(-0.1f, transform.position.y, transform.position.z);
        }

        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        float timeElapsed = 0;

        while (timeElapsed < moveTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, timeElapsed / moveTime);
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        noteController.setLastDelay((float)(hittingController.audioEngine.getTime() % (1000 / BPS)));
        Destroy(gameObject);
    }
}
