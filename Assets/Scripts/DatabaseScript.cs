using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseScript : MonoBehaviour
{
    IDatabaseRepository databaseRepository;
    public void sampleCollected(Sample sample)
    {
        databaseRepository.addSample(sample);
    }
}
