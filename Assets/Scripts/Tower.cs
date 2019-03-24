using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Tower : MonoBehaviour
{
    public int Cost;
    public Canvas optionsCanvas;
    public enum PowerState{FULL, PARTIAL, OFF};
    public PowerState powerState;
    public int FullPowerThreshhold;
    public int LowPowerThreshhold;

    public GameObject projectile;
    private Transform firePoint;
    private int curEnergyCharge;
    private int curUpgradeStage = 0;    
    private int[] upgradeCosts = new int[3];
    private float fireDelay = 0;

    #region Range Detection
    public List<GameObject> inRange;
    private SphereCollider range;

    private enum Targeting {FIRST, MIDDLE, LAST};
    private Targeting targetingChocie = Targeting.FIRST;

    #endregion
    void Start()
    {
        inRange = new List<GameObject>();
        this.GetComponent<MeshRenderer>().enabled = false;
        range = this.GetComponent<SphereCollider>();
        //TODO: Set radius here
        ToggleOptions();
        upgradeCosts =  new int[]{(Cost), (Cost * 3), (Cost * 3)};
        firePoint = this.transform.Find("FirePoint").gameObject.transform;
    }
    public void Spawn()
    {
        this.GetComponent<MeshRenderer>().enabled = true;
    }

#region Updates
    public virtual void Update()
    {
        DetectTouch();

            HandleAttack();
    }

    private void DetectTouch()
    {
        RaycastHit hit; 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);             
        if ( Physics.Raycast (ray,out hit,100.0f)) 
        {
            if(Input.GetMouseButtonDown(0) && hit.transform == this.transform)
            {
                ToggleOptions();
            }
        }  
    }
   
   private void HandleAttack()
   {
       if(fireDelay < 0.0f)
       {
        if(inRange.Count > 0)
            {
                GameObject target = GetTarget();
                if(target != null)
                {
                GameObject spawnedObj = Instantiate(projectile, firePoint.position, firePoint.rotation, null);
                spawnedObj.GetComponent<Projectile>().target = target;
                fireDelay = projectile.GetComponent<Projectile>().fireRate;
                }
            }            
       }
       else
       {
           fireDelay -= Time.deltaTime;
       }
   }


    public void UpdateEnergy(int newValue)
    {
        curEnergyCharge = newValue;
        if(curEnergyCharge > FullPowerThreshhold)
        {
            powerState = PowerState.FULL;
            this.GetComponent<MeshRenderer>().material.color = Color.cyan;
        }
        else if(curEnergyCharge < LowPowerThreshhold)
        {
            powerState = PowerState.OFF;
            this.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else
        {
            powerState = PowerState.PARTIAL;
            this.GetComponent<MeshRenderer>().material.color = Color.grey;
        }            
    }

#endregion
  
#region Buttons
    public virtual void ToggleOptions()
    {
        //optionsCanvas.transform.LookAt(Camera.main.transform);
        optionsCanvas.enabled = !optionsCanvas.enabled;
    }
    public virtual void Destroy()
    {
        PowerSource source = FindObjectOfType<PowerSource>();
        source.Towers.Remove(this.gameObject);
        source.UpdatePower();
        Destroy(this.gameObject);
    }

    public virtual void Upgrade()
    {
        Debug.Log("UPGRADED");
        PowerSource source = FindObjectOfType<PowerSource>();
        if(source.currency > upgradeCosts[curUpgradeStage])
        {
            source.SpendPlayerCurrency(upgradeCosts[curUpgradeStage]);
            curUpgradeStage++;
        }
        else
        {
            //Invalid Action
        }

    }

#endregion

 private GameObject GetTarget()
   {
       int targetNode = 0;
       GameObject target = null;
       
       switch(targetingChocie)
       {
        case Targeting.FIRST:
        {
            foreach(GameObject enemy in inRange)
            {
                Enemy temp = enemy.GetComponent<Enemy>();
                if(temp.GetNodeIndex() > targetNode)
                {
                    target = enemy;
                    targetNode = temp.GetNodeIndex();
                }
            }
            break;
        }
        case Targeting.MIDDLE:
        {
            target = null;
            break;
        }
        case Targeting.LAST:
        {
            target = null;
            break;
        }
       }
        return target;
   }

//Subscribed to EnemyDeathDelegate (val = worth, obj = Object)
    public void OnEnemyKilled(int val, GameObject obj)
    {
        inRange.Remove(obj);
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            inRange.Add(other.transform.parent.gameObject);
            other.transform.parent.GetComponent<Enemy>().deathEvent += OnEnemyKilled;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Enemy")
        {
            inRange.Remove(other.gameObject.transform.parent.gameObject);
            other.transform.parent.GetComponent<Enemy>().deathEvent -= OnEnemyKilled;
        }
    }
}