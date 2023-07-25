using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{
    IDatabaseRepository databaseRepository;
    public TextMeshProUGUI totalSampleText;
    public TextMeshProUGUI currentSampleText;
    public TextMeshProUGUI batteryText;
    public Slider batterySlider;
    int batteryLife = 5000;
    bool batteryDraining;
    int currentSamplesCollected = 0;


    void Start()
    {
        SetupDatabase();
        ShowSamplesCollected();
    }

    private void FixedUpdate()
    {
        if (batteryDraining)
        {
            batteryLife -= 1;
            if (batteryLife % 10 == 0)
            {
                UpdateBatteryLifeUI();
            }
        }
    }

    void SetupDatabase()
    {
        string databasePath = Application.persistentDataPath + "/main_db.sqlite";
        if (!System.IO.File.Exists(databasePath))
        {
            SqliteDatabase.CreateDatabase(databasePath);
        }
        databaseRepository = new SqliteDatabase("URI=file:" + databasePath);
        try
        {
            databaseRepository.switchUser(1);
        }
        catch (SqliteException)
        {
            databaseRepository.createUser();
        }
    }

    private void ShowSamplesCollected()
    {
        List<Sample> samples = databaseRepository.samples();
        foreach (Sample sample in samples)
        {
            if (sample.id == 0)
            {
                // TODO: Maybe just one sample is enough?
                totalSampleText.text = sample.quantity.ToString();
                currentSampleText.text = currentSamplesCollected.ToString();
            }
        }
    }

    public void SampleCollected(Sample sample)
    {
        databaseRepository.addSample(sample);
        currentSamplesCollected++;
        ShowSamplesCollected();
    }

    public void UpdateBatteryLifeUI()
    {
        int batteryLifePercent = batteryLife / 50;
        if (batteryLifePercent <= 0)
        {
            batteryLifePercent = 0;
            GameOver();
        }
        batteryText.text = batteryLifePercent.ToString();
        batterySlider.value = batteryLifePercent; 
    }

    public void SetBatteryDraining(bool draining)
    {
        batteryDraining = draining;
    }
    public void DrainBattery(int amount)
    {
        batteryLife = batteryLife - amount;
        UpdateBatteryLifeUI();
    }

    public void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
