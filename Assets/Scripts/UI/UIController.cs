using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController main;

    Settings settings;

    [Header("To Link")]
    public Transform worldCanvas;
    public GameObject incomeIndicatorPrefab;
    
    public Text moneyIndicator;
    public Text speedIndicator;
    //public Text priceIndicator;

    public ShopWindowController upgradesWindow;
    public ShopWindowController creaturesWindow;
    public CongratsWindow congratsWindow;


    [Header("Properties")]

    public decimal money, incomeSpeed;
    public decimal price;

    private void Awake()
    {
        main = this;
    }

    void Start()
    {
        settings = Settings.main;
        Debug.Log("UI Start");
    }

    void Update()
    {
        moneyIndicator.text = AlteredStringForm(money) + Money.symbol;
        speedIndicator.text = AlteredStringForm(incomeSpeed) + Money.symbol + "/s";
    }

    public void ShowIncome(decimal income, Vector3 position)
    {
        var indicator = Instantiate(incomeIndicatorPrefab, worldCanvas);
        indicator.transform.position = position;

        indicator.GetComponentInChildren<Text>().text = "+" + AlteredStringForm(income) + Money.symbol;
    }

    public static string AlteredStringForm(decimal number, string separator = "")
    {
        int lastIndex = 0;
        decimal alteredNumber = number;
        for (int i = 1; i < Money.prefixes.Length; i++)
        {
            if (number < Money.DecimalPow(Money.HIGH_NUMBER, i))
            {
                lastIndex = i - 1;
                alteredNumber = number / Money.DecimalPow(Money.HIGH_NUMBER, lastIndex);
                break;
            }
        }

        string alteredString = alteredNumber.ToString();
        int maxLength = Mathf.Min(5, alteredNumber.ToString().Length);

        return alteredString.Substring(0,maxLength) + separator + Money.prefixes[lastIndex].shortName;
    }

    public void ToggleWindow(ShopWindowController window)
    {
        if (window.gameObject.activeInHierarchy)
            HideWindow(window);
        else
            ShowWindow(window);
    }


    public void ShowWindow(Window window)
    {
        CreatureController.canCreturesBeMoved = false;
        window.gameObject.SetActive(true);
        window.Refresh();
    }

    public void HideWindow(Window window)
    {
        CreatureController.canCreturesBeMoved = true;
        window.gameObject.SetActive(false);
    }
}
