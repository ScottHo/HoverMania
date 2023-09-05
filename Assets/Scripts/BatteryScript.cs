using UnityEngine;

public class BatteryScript : MonoBehaviour
{
    LevelLogicScript logic;
    bool beingDestroyed = false;
    bool movingUp = true;
    Vector3 down, up;
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("LevelLogic").GetComponent<LevelLogicScript>();
        down = transform.position;
        up = transform.position;
        up.y += .4f;
    }

    void FixedUpdate()
    {
        float speed = .65f;
        float step = speed * Time.deltaTime;
        if (movingUp)
        {
            transform.position = Vector3.MoveTowards(transform.position, up, step);
            if (Vector3.Distance(transform.position, up) < 0.001f)
            {
                movingUp = false;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, down, step);
            if (Vector3.Distance(transform.position, down) < 0.001f)
            {
                movingUp = true;
            }
        }

        Vector3 eulers = transform.rotation.eulerAngles;
        eulers.z += 2.0f;
        transform.rotation = Quaternion.Euler(eulers);
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
