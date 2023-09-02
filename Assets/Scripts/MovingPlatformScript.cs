using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformScript : MonoBehaviour
{
    public List<Vector3> points;
    public float speed = 3.0f;
    int currentIdx = 0;
    bool arrived = false;
    float stallTimeStart = 0;
    float stallTime = .5f;

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
        transform.position = Vector3.MoveTowards(transform.position, points[currentIdx], step);

        if (Vector3.Distance(transform.position, points[currentIdx]) < 0.001f)
        {
            currentIdx += 1;
            if (currentIdx >= points.Count) { currentIdx = 0; }
            arrived = true;
        }
    }
}
