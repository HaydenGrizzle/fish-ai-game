using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemy;
    public Vector3 boxSize = new Vector3(500, 50, 500);
    public float spawnInterval = 2f;
    public int maxPellets = 50;

    private float timer;
    private int currentEnemy;
    private Terrain terrain;

    void Start()
    {
        terrain = Terrain.activeTerrain; // Get your terrain automatically
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval && currentEnemy < maxPellets)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }


    void SpawnEnemy()
    {
        int rand = Random.Range(0, enemy.Length);

        Vector3 randomPos = new Vector3(
            transform.position.x + Random.Range(-boxSize.x / 2f, boxSize.x / 2f),
            transform.position.y,
            transform.position.z + Random.Range(-boxSize.z / 2f, boxSize.z / 2f)
        );

        // Adjust Y based on terrain height
        if (terrain != null)
        {
            float terrainHeight = terrain.SampleHeight(randomPos);
            randomPos.y = terrainHeight + Random.Range(0.5f, 67f); // slightly above ground
        }

        Instantiate(enemy[rand], randomPos, Quaternion.identity);
        currentEnemy++;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}
