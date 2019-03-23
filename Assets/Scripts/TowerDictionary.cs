using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDictionary : MonoBehaviour
{    
    static private int TOWERARRAYSIZE = 1;
    [SerializeField]
    public GameObject[] Towers;

    public GameObject GetTower(int key)
    {
        if(Towers[key] != null)
        {
            return Towers[key];
        }
        return null;
    }
}
