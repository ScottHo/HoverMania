using UnityEngine;
using UnityEngine.UI;

public class SplashScript : MonoBehaviour
{
    public RawImage image;
    public float duration;
    public AudioSource audioSource;
    bool fadingIn = true;

    private void Start()
    {
        image.color = Color.black;
    }
    void FixedUpdate()
    {
        if (fadingIn)
        {
            FadeIn();
        }
        else
        {
            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                FadeOut();
            }
        }
    }

    void FadeIn()
    {
        Color color = image.color;
        color.b += .05f;
        color.r += .05f;
        color.g += .05f;
        image.color = color;
        if (color.b >= 1)
        {
            fadingIn = false;
        }
    }
    
    void FadeOut()
    {
        Color color = image.color;
        color.b -= .05f;
        color.r -= .05f;
        color.g -= .05f;
        image.color = color;
        if (color.b <= 0)
        {
            audioSource.Play();
            Destroy(gameObject);
        }
    }
}
