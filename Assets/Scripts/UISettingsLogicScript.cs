using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISettingsLogicScript : MonoBehaviour
{
    public Slider musicSlider;
    public Slider effectsSlider;
    public Slider ambienceSlider;
    public TMP_Dropdown graphicsDropdown;
    public GameObject container;
    public GameObject offlineContainer;
    public GameObject changeUserContainer;
    public Button changeUsernameButton;

    // Start is called before the first frame update
    void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("VolumeMusic");
        effectsSlider.value = PlayerPrefs.GetFloat("VolumeEffects");
        ambienceSlider.value = PlayerPrefs.GetFloat("VolumeAmbience");
        graphicsDropdown.value = PlayerPrefs.GetInt("Graphics");

    }

    public void Show()
    {
        container.SetActive(true);
        if (PlayerPrefs.GetInt("LeaderboardConnected") == 0)
        {
            changeUsernameButton.gameObject.SetActive(false);
        }
    }

    public void Hide()
    {
        container.SetActive(false);
    }

    public void ChangeUser()
    {
        changeUserContainer.SetActive(true);
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
