using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerData : MonoBehaviour
{
    public int Currency;
    public Canvas UICanvas;
    public TextMeshProUGUI currencyText;


    void Start()
    {
         Currency = 100;
         currencyText.text = Currency.ToString();
    }


    public void AdjustCurrency(bool isPositive, int value)
    {
        if(isPositive)
            Currency += value;
        else
            Currency -= value;

        currencyText.text = Currency.ToString();
    }


    public void TEMPAddCurrencyBtn()
    {
        AdjustCurrency(true, 50);
    }

    public void OnEnemyKilled(int val, GameObject obj)
    {
        AdjustCurrency(true, val);
    }
}