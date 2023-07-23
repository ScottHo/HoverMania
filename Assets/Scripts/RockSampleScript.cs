using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSampleScript : MonoBehaviour
{
    Sample sample;
    public DatabaseScript database;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
           database.sampleCollected(sample);
        }
        Destroy(gameObject);
    }
}
