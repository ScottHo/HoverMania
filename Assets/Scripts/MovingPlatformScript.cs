using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformScript : MonoBehaviour
{
    public List<Vector3> points;
    public float speed = 2.5f;
    int currentIdx = 0;
    bool arrived = false;
    float stallTimeStart = 0;
    float stallTime = .5f;
    bool entered = false;

    void FixedUpdate()
    {
        if (arrived)
        {
            stallTimeStart += Time.deltaTime;
            if (stallTimeStart > stallTime)
            {
                stallTimeStart = 0;
                arrived = false;
            }
            return;
        }
        float step = speed * Time.deltaTime;
        transform.parent.position = Vector3.MoveTowards(transform.parent.position, points[currentIdx], step);

        if (Vector3.Distance(transform.parent.position, points[currentIdx]) < 0.001f)
        {
            currentIdx += 1;
            if (currentIdx >= points.Count) { currentIdx = 0; }
            arrived = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (entered)
            return;
        if (other.gameObject.CompareTag("Player"))
        {
            entered = true;
            other.gameObject.transform.parent = transform.parent;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!entered)
        {
            return;
        }
        if (other.gameObject.CompareTag("Player"))
        {
            entered = false;
            other.gameObject.transform.parent = null;
        }
    }
}
