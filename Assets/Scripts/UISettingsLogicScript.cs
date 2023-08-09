using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISettingsLogicScript : MonoBehaviour
{
    public Slider musicSlider;
    public Slider effectsSlider;
    public Slider ambienceSlider;
    public TMP_Dropdown graphicsDropdown;

    // Start is called before the first frame update
    void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("VolumeMusic");
        effectsSlider.value = PlayerPrefs.GetFloat("VolumeEffects");
        ambienceSlider.value = PlayerPrefs.GetFloat("VolumeAmbience");
        graphicsDropdown.value = PlayerPrefs.GetInt("Graphics");
    }

    public void UpdateMusicSlider()
    {
        PlayerPrefs.SetFloat("VolumeMusic", musicSlider.value);
    }
    public void UpdateEffectsSlider()
    {
        PlayerPrefs.SetFloat("VolumeEffects", effectsSlider.value);
    }

    public void UpdateAmbienceSlider()
    {
        PlayerPrefs.SetFloat("VolumeAmbience", ambienceSlider.value);
    }

    public void UpdateGraphics()
    {
        PlayerPrefs.SetInt("Graphics", graphicsDropdown.value);
        QualitySettings.SetQualityLevel(graphicsDropdown.value);
    }
}
