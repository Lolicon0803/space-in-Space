using System.Collections;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    private float movementSpeed;
    private HittingController hittingController;
    private float BPS;
    private float moveTime;

    // Start is called before the first frame update
    void Start()
    {
        hittingController = GameObject.Find("HittingUI").GetComponent<HittingController>();
        BPS = (float)(hittingController.audioEngine.BPM / 60d);
        moveTime = BPS * 2;
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        float timeElapsed = 0;

        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(0, transform.position.y, transform.position.z);

        while (timeElapsed < moveTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, timeElapsed / moveTime);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        Destroy(gameObject);
    }
}
