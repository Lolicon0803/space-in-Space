using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleAnime : MonoBehaviour
{
    // Start is called before the first frame update
    float[] speed = new float[12];
    bool[] canMove = new bool[12];

    void Start()
    {
        for (int i = 0; i < 12; i++)
        {
            speed[i] = 0.05f;
            canMove[i] = false;
        }
        canMove[0] = true;
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < 12; i++)
        {
            Vector3 newVec = gameObject.transform.GetChild(3 + i).transform.position;

            if (gameObject.transform.GetChild(2 + i).transform.position.y > 350)
            {
                canMove[i] = true;
            }

            if (canMove[i])
            {
                gameObject.transform.GetChild(3 + i).transform.position = new Vector3(newVec.x, newVec.y + speed[i], newVec.z);
            }

            if (gameObject.transform.GetChild(3 + i).transform.position.y > 385 ||
                gameObject.transform.GetChild(3 + i).transform.position.y < 310)
            {
                speed[i] = -speed[i];
            }

        }

    }
}
