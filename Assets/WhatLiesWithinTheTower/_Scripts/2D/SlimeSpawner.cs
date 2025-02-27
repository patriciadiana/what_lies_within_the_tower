using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSpawner : MonoBehaviour
{
    public GameObject slime;
    public int minSlimesPerCoffin = 1;
    public int maxSlimesPerCoffin = 2;
    public float spawnRadius = 2f;

    void Start()
    {
        SpawnSlimesNearCoffins();
    }

    void SpawnSlimesNearCoffins()
    {
        GameObject[] coffins = GameObject.FindGameObjectsWithTag("coffin");

        foreach (GameObject coffin in coffins)
        {
            int slimesToSpawn = Random.Range(minSlimesPerCoffin, maxSlimesPerCoffin + 1);

            for (int i = 0; i < slimesToSpawn; i++)
            {
                Vector2 randomPosition = (Vector2)coffin.transform.position +
                                         Random.insideUnitCircle * spawnRadius;

                Instantiate(slime, randomPosition, Quaternion.identity);
            }
        }
    }
}
