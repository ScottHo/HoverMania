using UnityEngine;

public class BatteryScript : MonoBehaviour
{
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
                logic.IncreaseBatteryLife(2500);
                Destroy(gameObject);
            }
        }
    }
}
