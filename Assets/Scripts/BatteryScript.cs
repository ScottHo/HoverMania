using UnityEngine;

public class BatteryScript : MonoBehaviour
{
    public GameObject textPrefab;
    public MeshRenderer meshRenderer;
    LevelLogicScript logic;
    bool beingDestroyed = false;
    bool movingUp = true;
    float textTimerStart = 0.0f;
    float textTimer = 2.0f;
    Quaternion textRotation;
    Vector3 down, up;
    GameObject textObject;
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("LevelLogic").GetComponent<LevelLogicScript>();
        down = transform.position;
        up = transform.position;
        up.y += .4f;
    }

    void FixedUpdate()
    {
        if (beingDestroyed)
        {
            if (textTimerStart == 0.0f)
            {
                meshRenderer.forceRenderingOff = true;
                textObject = Instantiate(textPrefab, gameObject.transform.position, textRotation);
            }
            textTimerStart += Time.deltaTime;
            if (textTimer < textTimerStart)
            {
                Destroy(textObject);
                Destroy(gameObject);
            }
            return;
        }
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
                logic.IncreaseBatteryLife(3000);
                textRotation = collision.gameObject.transform.rotation;
            }
        }
    }
}
