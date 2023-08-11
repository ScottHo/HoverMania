using UnityEngine;

public class RockSampleScript : MonoBehaviour
{
    int sampleID;
    LevelLogicScript logic;
    bool beingDestroyed = false;
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("LevelLogic").GetComponent<LevelLogicScript>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 3)
        {
            if (!beingDestroyed)
            {
                beingDestroyed = true;
                logic.SampleCollected(SampleFactory.createSample(sampleID, 1));
                Destroy(gameObject);
            }
        }
    }
}
