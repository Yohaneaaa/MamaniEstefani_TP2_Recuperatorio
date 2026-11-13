using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemySpawner : MonoBehaviour
{

    public GameObject enemyPrefab;

    private float startDelay;  
    private float interval;   
    private int maxAlive;   
    private List<GameObject> spawned;

    private void Start()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("EnemySpawner: falta asignar el prefab del monstruo.");
            return;
        }

        startDelay = 5f;
        interval = 10f;
        maxAlive = 3;
        spawned = new List<GameObject>();

        InvokeRepeating(nameof(SpawnEnemy), startDelay, interval);
    }

    private void SpawnEnemy()
    {
        spawned.RemoveAll(enemy => enemy == null);
        if (spawned.Count >= maxAlive) return;

        NavMeshHit hit;
        if (!NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
        {
            Debug.LogWarning("EnemySpawner: no hay NavMesh a menos de 10 unidades. Mover el spawner a una zona celeste.");
            return;
        }

        GameObject enemy = Instantiate(enemyPrefab, hit.position, transform.rotation);
        spawned.Add(enemy);
        Debug.Log("Spawner: nuevo monstruo generado (" + spawned.Count + "/" + maxAlive + ")");
    }
}