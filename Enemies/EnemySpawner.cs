/* Written by Cameron Williams
 * 
 * This script implements a timer based enemy spawner that counts down via
 * a CoRoutine and the WaitForSeconds() method. Recently added functionality for
 * specifiying the number of waves to spawn as well as an OnDrawGizmosSelected()
 * function for visualizing the spawn area.
 * 
 * Last updated 8/21/2019
 */

using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] [Tooltip("Amount of time to wait before spawning a new wave")] private int timeBetweenWaves = 30;
    [SerializeField] [Tooltip("Enemy type that this spawner will create")] private GameObject enemyPrefab = null;
    [SerializeField] [Tooltip("Number of enemies to spawn in a single wave")] private int enemiesPerWave = 5;
    [SerializeField] [Tooltip("Number of waves to spawn")] private int wavesToSpawn = 3;
    [SerializeField] [Tooltip("Possible area along the x-axis in which enemies may be instantiatd by the spawner")] private int spawnAreaXVariability = 50;
    [SerializeField] [Tooltip("Possible area along the y-axis in which enemies may be instantiatd by the spawner")] private int spawnAreaYVariability = 5;
    [SerializeField] [Tooltip("Possible area along the z-axis in which enemies may be instantiatd by the spawner")] private int spawnAreaZVariability = 20;
    // Need to make the spawn timer count faster when player is mining

    void OnDrawGizmosSelected() // visualization of spawn area
    {
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, transform.forward);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position - new Vector3(spawnAreaXVariability, 0, 0), transform.position + new Vector3(spawnAreaXVariability, 0, 0));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position - new Vector3(0, spawnAreaYVariability, 0), transform.position + new Vector3(0, spawnAreaYVariability, 0));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position - new Vector3(0, 0, spawnAreaZVariability), transform.position + new Vector3(0, 0, spawnAreaZVariability));
    }

    void Start()
    {
        StartCoroutine(SpawnTimer(timeBetweenWaves, enemiesPerWave)); // Starts the spawn timer, sending the time and enemies per wave as parameters
    }

    private IEnumerator SpawnTimer(int time, int enemies) // the main spawn coroutine that manages the countdown and spawns a specified number of enemies each wave
    {
        for (int i = 0; i < wavesToSpawn; i++)
        {
            yield return new WaitForSeconds(time);
            StartCoroutine(SpawnEnemies());
        }
    }

    private IEnumerator SpawnEnemies() // the function that actually instantiates enemies near the spawner within a semi-random area specified by the variability ints
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            yield return new WaitForSeconds(2.0f); // wait for a short amount of time so prefabs don't overlap
            Vector3 spawnPostion = transform.position + new Vector3(Random.Range(-spawnAreaXVariability, spawnAreaXVariability),
                                                                    Random.Range(-spawnAreaYVariability, spawnAreaYVariability),
                                                                    Random.Range(-spawnAreaZVariability, spawnAreaZVariability)); // randomize spawn position around spawner
            Quaternion spawnRotation = transform.rotation; // spawn prefabs facing the same way as the spawner
            Instantiate(enemyPrefab, spawnPostion, spawnRotation);
        }
    }
}
