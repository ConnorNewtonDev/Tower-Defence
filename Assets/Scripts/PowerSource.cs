using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PowerSource : MonoBehaviour
{
private TowerDictionary towerDict;
private PlayerData player;
public List<GameObject> Towers = new List<GameObject>();
private GameObject socketTower;
private int Energy = 100;
public int curEnergy;
public TextMeshProUGUI energyText;
public int currency = 0;
private Vector2 mousePos;
private LayerMask inputLayerMasks;
void Start()
{
    player = FindObjectOfType<PlayerData>();
    towerDict = FindObjectOfType<TowerDictionary>();
    UpdateUIText(curEnergy = Energy);
}

// Update is called once per frame
void Update()
{
    if(socketTower != null)
    {           
        HandleTowerPlacement();
    }
}

void UpdateUIText(int val)
{
    energyText.text = val.ToString() + " / " + Energy.ToString();
}

private void HandleTowerPlacement()
{
    if(socketTower != null)
    {
        LayerMask layerMask = ~(1 << 10);
        RaycastHit hit; 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);             
        if ( Physics.Raycast (ray,out hit,100.0f, layerMask)) 
        {
            socketTower.transform.position = hit.point + new Vector3(0, 1, 0);
            
        
            //Handle placement attempt
            if(Input.GetMouseButtonDown(0))
            {
                socketTower.transform.position = hit.point + new Vector3(0, 1, 0);            
                //If tower is in a valis spawn position
                if(socketTower.GetComponent<Tower>().Spawn())
                {
                    Towers.Add(socketTower);        
                    UpdateUIText(curEnergy = UpdatePower());           
                    socketTower = null;
                }
            }
        }
    }
}

public void SpendPlayerCurrency(int value)
{
    player.AdjustCurrency(false, value);
}

public void TowerSpawnBtn()
{
    if(socketTower == null)
        NewTowerAction(0);          //Basic Tower
}

public int UpdatePower()
{
    if(Towers.Count == 0)
    {
        return Energy;          //Return Full Energy
    }


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
        if(Cost <= player.Currency)
        {
            player.AdjustCurrency(false, Cost);
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
