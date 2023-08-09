using UnityEngine;

public class SettingsPanelScript : MonoBehaviour
{
    float target = 2000;
    bool deactivate = false;
    public void SlideIn()
    {
        gameObject.SetActive(true);
        target = -80f;
        deactivate = false;
    }
    public void SlideOut()
    {
        target = 1840f;
        deactivate = true;
    }
    void FixedUpdate()
    {
        Vector3 pos = gameObject.transform.position;
        if (pos.x == target)
        {
            if (deactivate)
            {
                gameObject.SetActive(false);
            }
            return;
        }
        if (target == -80f)
        {
            pos.x -= 80f;
        }
        else if (target == 1840f)
        {
            pos.x += 80f;
        }
        gameObject.transform.position = pos;
    }
}
