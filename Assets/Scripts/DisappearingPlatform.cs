using System.Collections.Generic;
using UnityEngine;

public class DisappeartingPlatform : MonoBehaviour

{
    public List<int> blinkCycle;
    public float solidTime = 4f;
    int index = 0;
    float current = 0;
    float startR = .2f;
    float startG = .44f;
    float startB = .85f;
    float endR = .04f;
    float endG = .12f;
    float endB = .25f;

    private void Start()
    {
        Blink();
    }
    private void FixedUpdate()
    {
        current += Time.deltaTime;
        int nextIdx = index + 1;
        nextIdx %= blinkCycle.Count;
        if (current < solidTime-.5f)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = new Color(startR, startG, startB);
        }
        else if (current < solidTime && blinkCycle[nextIdx] == 0)
        {
            float coef = (solidTime - current) * 2;
            float r = endR + (startR - endR) * coef;
            float g = endG + (startG - endG) * coef;
            float b = endB + (startB - endB) * coef;


            gameObject.GetComponent<MeshRenderer>().material.color = new Color(r, g, b);
        }
        else if (current >= solidTime)
        {
            index = nextIdx;
            current = 0;
            Blink();
        }
    }
    void Blink()
    {
        if (blinkCycle[index] == 1)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            gameObject.GetComponent<BoxCollider>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
