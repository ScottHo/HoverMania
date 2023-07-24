using Mono.Data.Sqlite;
using UnityEngine;
using TMPro;
using System.Collections.Generic;


public class DatabaseScript : MonoBehaviour
{
    IDatabaseRepository databaseRepository;
    public TextMeshProUGUI sampleText;

    // Start is called before the first frame update
    void Start()
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
        } catch (SqliteException e)
        {
            databaseRepository.createUser();
        }
        showSamplesCollected();
    }

    private void showSamplesCollected()
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

    public void sampleCollected(Sample sample)
    {
        databaseRepository.addSample(sample);
        showSamplesCollected();
    }
}
