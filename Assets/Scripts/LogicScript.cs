using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;


public class LogicScript : MonoBehaviour
{
    IDatabaseRepository databaseRepository;
    public TextMeshProUGUI sampleText;
    public TextMeshProUGUI batteryText;
    public Slider batterySlider;
    int batteryLife = 5000;
    bool batteryDraining;


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
                sampleText.text = sample.quantity.ToString();
            }
        }
    }

    public void SampleCollected(Sample sample)
    {
        databaseRepository.addSample(sample);
        ShowSamplesCollected();
    }

    public void UpdateBatteryLifeUI()
    {
        int batteryLifePercent = batteryLife / 50;
        batteryText.text = batteryLifePercent.ToString();
        batterySlider.value = batteryLifePercent; 
    }

    public void SetBatteryDraining(bool draining)
    {
        batteryDraining = draining;
    }
}
