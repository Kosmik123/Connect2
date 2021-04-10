using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController main;

    [Header("To Link")]
    public Transform worldCanvas;
    public GameObject incomeIndicatorPrefab;
    
    public Text moneyIndicator;
    public Text speedIndicator;
    public Text priceIndicator;

    public decimal money, incomeSpeed;
    public decimal price;

    private void Awake()
    {
        main = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        moneyIndicator.text = AlteredStringForm(money) + "$";
        speedIndicator.text = AlteredStringForm(incomeSpeed) + "$/s";
        priceIndicator.text = AlteredStringForm(price) + "$";
    }

    public void ShowIncome(decimal income, Vector3 position)
    {
        var indicator = Instantiate(incomeIndicatorPrefab, worldCanvas);
        indicator.transform.position = position;


        indicator.GetComponentInChildren<Text>().text = "+" + AlteredStringForm(income) + "$";
    }

    public static string AlteredStringForm(decimal number, string separator = "")
    {
        int lastIndex = 0;
        decimal alteredNumber = number;
        for (int i = 1; i < Money.prefixes.Length; i++)
        {
            if (number < (decimal)System.Math.Pow(Money.HIGH_NUMBER, i))
            {
                lastIndex = i - 1;
                alteredNumber = number / (decimal)System.Math.Pow(Money.HIGH_NUMBER, lastIndex);
                break;
            }
        }

        string alteredString = alteredNumber.ToString();
        int maxLength = Mathf.Min(5, alteredNumber.ToString().Length);

        return alteredString.Substring(0,maxLength) + separator + Money.prefixes[lastIndex].shortName;
    }


}
