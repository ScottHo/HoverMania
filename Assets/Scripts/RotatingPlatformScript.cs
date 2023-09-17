using UnityEngine;

public class RotatingPlatformScript : MonoBehaviour
{
    Vector3 eulers = Vector3.zero;
    bool entered = false;

    void FixedUpdate()
    {
        eulers.y += 1.0f;
        transform.parent.rotation = Quaternion.Euler(eulers);
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
