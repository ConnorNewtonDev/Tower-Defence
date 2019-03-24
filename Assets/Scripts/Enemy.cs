using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
#region Stats
[SerializeField]
    private int Health;
    private int Worth;
    public float moveSpeed;
    public float moveMofifier = 1.0f;
    private GameObject Object;
#endregion

#region Navigation
    private NavMeshAgent navComponent;
    public Transform[] nodes;
    private int nodeIndex = 0;
    
#endregion

    public delegate void DeathDelegate(int val, GameObject obj);
    public DeathDelegate deathEvent;

  
  
    public void Spawned(EnemyData data, Transform[] path)
    {
        moveSpeed = data.Speed;
        Health = data.Health;
        Worth = data.Worth;
        Object = Instantiate(data.Object, this.transform.position, this.transform.rotation, this.transform);  
        nodes = path;

    }

    // Update is called once per frame
    void Update()
    {
        if(nodes.Length > 0)
        Move();
    }

    public void Damage(int dmg)
    {
        if(Health - dmg <= 0)
        {
            Died();
        }
        else
        {
            Health -= dmg;
        }
    }

    private void Move()
    {
        Vector3 nodePos = new Vector3(nodes[nodeIndex].position.x, 1.5f, nodes[nodeIndex].position.z);
        Vector3 direction = nodePos - this.transform.position; 
        if(Vector3.Distance(this.transform.position, nodePos) > 0.2f)
        {
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(direction * moveSpeed * Time.deltaTime);
            if(nodeIndex +1 == nodes.Length)
            {
                Survived();
            }
            nodeIndex++;
        } 
    }

    public int GetNodeIndex()
    {
        return nodeIndex;
    }
    public void Died()
    {            
        deathEvent(Worth, this.gameObject);
        Destroy(this.gameObject);
    }
    public void Survived() //When the Enemy reaches the end of the path
    {
        Destroy(this.gameObject);
    }

}
