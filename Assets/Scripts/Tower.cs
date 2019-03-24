using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Tower : MonoBehaviour
{
    public int Cost;
    private int curUpgradeStage = 0;
    public int curEnergyCharge;
    public int FullPowerThreshhold;
    public int LowPowerThreshhold;
    private float powerModifier = 0;
    public enum PowerState{FULL, PARTIAL, OFF};
    public PowerState powerState;
    public Canvas optionsCanvas;
    private int[] upgradeCosts = new int[3];
    
    
    void Start()
    {
        this.GetComponent<MeshRenderer>().enabled = false;
        ToggleOptions();
        upgradeCosts =  new int[]{(Cost), (Cost * 3), (Cost * 3)};
    }
    public void Spawn()
    {
        this.GetComponent<MeshRenderer>().enabled = true;
    }
    public virtual void Update()
    {
        DetectTouch();
    
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
    public void OnEnable()
    {
        
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


    public virtual void Fire()
    {}



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
            powerModifier += 0.5f;
        }
        else
        {
            //Invalid Action
        }

    }

#endregion





}