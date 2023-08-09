using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

public class InitScript : MonoBehaviour
{
    void Start()
    {
        CheckPrefs();
    }

    void CheckPrefs()
    {
        List<string> keys = new List<string>
        {
            "VolumeMusic",
            "VolumeEffects",
            "VolumeAmbience"
        };
        SetFloats(keys);
        if (!PlayerPrefs.HasKey("Graphics"))
        {
            PlayerPrefs.SetInt("Graphics", 1);
        }
    }
    void SetFloats(List<string> keys)
    {
        foreach (string key in keys)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetFloat(key, .5f);
            }
        }
    }
}
