using PlasticGui.Configuration.CloudEdition.Welcome;
using UnityEngine;

public class RockSampleScript : MonoBehaviour
{
    int sampleID;
    public DatabaseScript database;
    // Start is called before the first frame update
    void Start()
    {
        database = GameObject.FindGameObjectWithTag("Database").GetComponent<DatabaseScript>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 3)
        {
           database.sampleCollected(SampleFactory.createSample(sampleID, 1));
           Destroy(gameObject);
        }
    }
}
