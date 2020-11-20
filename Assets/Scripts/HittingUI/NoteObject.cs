using UnityEngine;

public class NoteObject : MonoBehaviour
{
    private float movementSpeed;
    private HittingController hittingController;
    private float BPS;

    // Start is called before the first frame update
    void Start()
    {
        hittingController = GameObject.Find("HittingUI").GetComponent<HittingController>();
        BPS = (float)(hittingController.audioEngine.BPM / 60d);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > 0)
        {
            transform.Translate(-BPS * Time.deltaTime * 1.1f, 0, 0);
            if (transform.position.x <= 0)
            {
                Destroy(gameObject);
            }
        }
        else if (transform.position.x < 0)
        {
            transform.Translate(BPS * Time.deltaTime * 1.1f, 0, 0);
            if (transform.position.x >= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
