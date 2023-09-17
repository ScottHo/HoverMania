using UnityEngine;

public class FollowScript : MonoBehaviour
{
    public GameObject car;
    // Update is called once per frame
    void Start()
    {
        transform.position = car.transform.position;
    }
    void FixedUpdate()
    {
        transform.position = car.transform.position;
    }
}
