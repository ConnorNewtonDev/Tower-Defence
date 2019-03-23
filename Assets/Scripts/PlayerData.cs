using System;
using UnityEngine;
using UnityEngine.UI;
public class PlayerData : MonoBehaviour
{
    public int Currency;
    public Canvas UICanvas;

    void Start()
    {
        Currency = 100;
    }


    public void AdjustCurrency(bool isPositive, int value)
    {
        if(isPositive)
            Currency += value;
        else
            Currency -= value;

        Debug.Log("Current Currency: " + Currency);
    }


    public void TEMPAddCurrencyBtn()
    {
        AdjustCurrency(true, 50);
    }

}