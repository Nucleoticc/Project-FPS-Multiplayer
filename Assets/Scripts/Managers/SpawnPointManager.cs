using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    public static SpawnPointManager instance;

    SpawnPoint[] spawnPoints;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        spawnPoints = GetComponentsInChildren<SpawnPoint>();
    }

    public Transform GetSpawnPoint()
    {
        SpawnPoint selectedSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
        while (!selectedSpawn.IsSpawnable)
        {
            selectedSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
        }

        return selectedSpawn.transform;
    }
}
