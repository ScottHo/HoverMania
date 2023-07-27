using UnityEngine;

public class RockSampleScript : MonoBehaviour
{
    int sampleID;
    LevelLogicScript logic;
    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("LevelLogic").GetComponent<LevelLogicScript>();
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
