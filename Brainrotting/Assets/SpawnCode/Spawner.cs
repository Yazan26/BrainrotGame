using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> Coins;
    public List<GameObject> Objects;
    public Transform player;
    public float spawnZDistance = 50f;
    public float distanceBetweenSpawns = 10f;

    private float nextSpawnZ;

    void Start()
    {
        nextSpawnZ = player.position.z + spawnZDistance;
    }

    void Update()
    {
        if (player.position.z + spawnZDistance > nextSpawnZ)
        {
            SpawnRow(nextSpawnZ);
            nextSpawnZ += distanceBetweenSpawns;
        }
    }

    void SpawnRow(float zPos)
    {
        for (int lane = 0; lane < 3; lane++)
        {
            int chance = Random.Range(0, 10);

            float xPos = (lane - 1) * 3f; // lanes at -3, 0, +3

            if (chance < 5)
            {
                // 50% chance: do nothing
                return;
            }
            else if (chance < 8)
            {
                // 30% chance: spawn cheese
                foreach (var coins in Coins)
                {
                    Instantiate(coins, new Vector3(xPos, coins.transform.position.y + 5, zPos), Quaternion.identity);
                }
            }
            else
            {
                // 20% chance: spawn skibidi toilet
                foreach (var objects in Objects)
                {
                    Instantiate(objects, new Vector3(xPos, objects.transform.position.y, zPos), Quaternion.identity);
                }
            }
        }
    }
}