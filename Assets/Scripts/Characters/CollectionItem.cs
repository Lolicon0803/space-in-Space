using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionItem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            GetComponent<SpriteRenderer>().color = new Vector4(0,0,0,0);
    
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
