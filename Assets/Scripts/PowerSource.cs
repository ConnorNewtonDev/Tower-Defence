using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSource : MonoBehaviour
{
private TowerDictionary towerDict;
public List<GameObject> Towers = new List<GameObject>();
private GameObject socketTower;
private int Energy = 100;
public int curEnergy;

public int currency = 0;
private Vector2 mousePos;
private LayerMask inputLayerMasks;
void Start()
{
    towerDict = FindObjectOfType<TowerDictionary>();
    curEnergy = Energy;
}

// Update is called once per frame
void Update()
{
    if(socketTower != null)
    {           
        HandleTowerPlacement();
    }
}

private void HandleTowerPlacement()
{
 if ( Input.GetMouseButtonDown (0))
    { 
        RaycastHit hit; 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);             
        if ( Physics.Raycast (ray,out hit,100.0f)) 
        {
            socketTower.transform.position = hit.point + new Vector3(0, 1, 0);
            socketTower.GetComponent<Tower>().Spawn();
            Towers.Add(socketTower);
            curEnergy = UpdatePower();

            socketTower = null;
        }           

    }
}

public void TowerSpawnBtn()
{
    NewTowerAction(0);          //Basic Tower
}

public int UpdatePower()
{
    if(Towers.Count == 0)
        return Energy;          //Return Full Energy

    int newVal = Energy / Towers.Count;
    foreach(GameObject tower in Towers)
    {   
        tower.GetComponent<Tower>().UpdateEnergy(newVal);
    }
    return newVal;
}

private void NewTowerAction(int towerID)
{
    GameObject spawnTower = towerDict.GetTower(towerID);

    if(spawnTower != null)
    {
        int Cost = spawnTower.GetComponent<Tower>().Cost;
        if(Cost <= currency)
        {
            currency -= Cost;
            SpawnTower(spawnTower);
        }
    }        
}

private void SpawnTower(GameObject spawnTower)
{
    Debug.Log("Spawning");
    GameObject temp = Instantiate(spawnTower, this.transform.position, this.transform.rotation) as GameObject;
    temp.transform.SetParent(this.transform);
    socketTower = temp;             
}

}
