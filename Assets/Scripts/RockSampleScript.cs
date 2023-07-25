using UnityEngine;

public class RockSampleScript : MonoBehaviour
{
    int sampleID;
    LogicScript logic;
    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 3)
        {
            logic.SampleCollected(SampleFactory.createSample(sampleID, 1));
           Destroy(gameObject);
        }
    }
}
