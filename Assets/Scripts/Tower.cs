using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Tower : MonoBehaviour
{

    public Canvas optionsCanvas;
    public enum PowerState{FULL, PARTIAL, OFF};
    public PowerState powerState;
    public int Cost;
    public int FullPowerThreshhold;
    public int LowPowerThreshhold;
    private float powerPercent;

    private bool placed = false;
    public GameObject projectile;
    private Transform firePoint;
    private int curEnergyCharge;
    private int curUpgradeStage = 0;    
    private int[] upgradeCosts = new int[3];
    private float fireDelay = 0;

    #region Range Detection
    public List<GameObject> inRange;
    private SphereCollider range;

    private int occupiedCount = 0;
    private enum Targeting {FIRST, MIDDLE, LAST};
    private Targeting targetingChocie = Targeting.FIRST;

    #endregion
    void Start()
    {
        inRange = new List<GameObject>();
        //this.GetComponent<MeshRenderer>().enabled = false;
        this.GetComponent<MeshRenderer>().material.color = Color.cyan;
        range = this.GetComponent<SphereCollider>();
        //TODO: Set radius here
        ToggleOptions();
        upgradeCosts =  new int[]{(Cost), (Cost * 3), (Cost * 3)};
        firePoint = this.transform.Find("FirePoint").gameObject.transform;
    }
    public bool Spawn()
    {
        if(occupiedCount > 0)
            return false;
        else
        {           
            placed = true;
            this.GetComponent<SphereCollider>().enabled = true;
            return true;
        }        

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


    public void UpdateEnergy(int newValue)          //TODO: Use powerstate switcher to set damage modifier & colour
    {
       // curEnergyCharge = newValue;
        powerPercent = (newValue /FullPowerThreshhold) * 100;
        if(powerPercent >= FullPowerThreshhold)
        {
            powerState = PowerState.FULL;
            this.GetComponent<MeshRenderer>().material.color = Color.cyan;
        }
        else if(powerPercent < LowPowerThreshhold)
        {
            powerState = PowerState.OFF;
            this.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else
        {
            powerState = PowerState.PARTIAL;
            this.GetComponent<MeshRenderer>().material.color = Color.magenta;
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

#region Combat
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


#endregion

#region Collision
    private void AdjustOccupied(int val)
    {
        if(placed)
            return;

        occupiedCount += val;
        if(occupiedCount != 0)
        {
            this.GetComponent<MeshRenderer>().material.color = Color.red;
        }        
        else
        {
            this.GetComponent<MeshRenderer>().material.color = Color.cyan;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch(other.tag)
        {
            case "Enemy":
                inRange.Add(other.transform.parent.gameObject);
                other.transform.parent.GetComponent<Enemy>().deathEvent += OnEnemyKilled;
            break;
            case "Occupied":
                if(other == other.GetComponent<BoxCollider>())
                    AdjustOccupied(1);
            break;
        }
        if(other.tag == "Enemy")
        {

        }
    }

    private void OnTriggerExit(Collider other)
    {        
        switch(other.tag)
        {
            case "Enemy":
                inRange.Remove(other.gameObject.transform.parent.gameObject);
                other.transform.parent.GetComponent<Enemy>().deathEvent -= OnEnemyKilled;
                break;
            case "Occupied":
                if(other == other.GetComponent<BoxCollider>())
                    AdjustOccupied(-1);
                break;
        }
    }

#endregion

}