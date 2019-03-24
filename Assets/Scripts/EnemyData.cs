using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "My Assets/Enemy Data")]
[System.Serializable]

public class EnemyData : ScriptableObject
{
    public int Health;
    public float Speed;
    public int Worth;
    public GameObject Object;
}
