using UnityEngine;

public class NoteObject : MonoBehaviour
{
    private float movementSpeed;
    private NoteController noteController;
    private float BPS;

    // Start is called before the first frame update
    void Start()
    {
        noteController = GameObject.Find("NoteHolder").GetComponent<NoteController>();
        movementSpeed = noteController.movementSpeed;
        BPS = (float)(noteController.tempoManager.BPM / 60d);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > 0)
        {
            transform.Translate(-BPS * Time.deltaTime * movementSpeed, 0, 0);
            if (transform.position.x <= 0)
            {
                Destroy(gameObject);
            }
        }
        else if (transform.position.x < 0)
        {
            transform.Translate(BPS * Time.deltaTime * movementSpeed, 0, 0);
            if (transform.position.x >= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
