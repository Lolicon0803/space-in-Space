using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleTest : MonoBehaviour
{
    public bool canIn;
    public BlackHoleTest exit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canIn)
        {
            StartCoroutine(Transport(collision.gameObject));
        }
    }

    private IEnumerator Transport(GameObject target)
    {
        yield return new WaitForSeconds(2f);
        target.transform.position = new Vector3(999, 999, 999);
        yield return null;
        target.transform.position = exit.transform.position;
    }
}
