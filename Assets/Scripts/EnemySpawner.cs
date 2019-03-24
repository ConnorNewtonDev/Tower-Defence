using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    private Transform path;
    public bool isSpawning = false;
    // Start is called before the first frame update
    public List<Transform> nodes = new List<Transform>();
    public List<SpawnListItem> spawns = new List<SpawnListItem>();
    public GameObject enemyPrefab;
    private float spawnDelay = 3;
    private Vector3 spawnPoint;
    private PlayerData pData;
    void Start()
    {
        path = GameObject.Find("Path").transform;
        pData = FindObjectOfType<PlayerData>();
        for(int i = 0; i < path.childCount; i++)
        {
            nodes.Add(path.GetChild(i).transform);
        }
        spawnPoint = new Vector3(nodes[0].transform.position.x, 1.5f, nodes[0].transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if(isSpawning)
        {
            if(spawnDelay  <=0)
            {
                GameObject enemy = Instantiate(enemyPrefab, spawnPoint, nodes[0].transform.rotation, null);
                enemy.GetComponent<Enemy>().Spawned(spawns[0].Data, nodes.ToArray());
                spawns.RemoveAt(0);
                enemy.GetComponent<Enemy>().deathEvent += pData.OnEnemyKilled;
                
                if(spawns.Count == 0)
                    ToggleSpawning();
                else
                    spawnDelay = spawns[0].spawnDelay;

            }
            else
            {
                spawnDelay -= Time.deltaTime;
            }
        }
    }

    public void ToggleSpawning()
    {
        isSpawning = !isSpawning;
    }

[System.Serializable]
    public struct SpawnListItem
    {
        public EnemyData Data;
        public float spawnDelay;
    }
}
