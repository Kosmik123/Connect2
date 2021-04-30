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
    //public Text priceIndicator;

    public ShopWindowController upgradesWindow;
    public ShopWindowController creaturesWindow;

    [Header("Properties")]

    public decimal money, incomeSpeed;
    public decimal price;

    private void Awake()
    {
        main = this;
    }

    void Start()
    {
        CreateCreatureProducts();
        creaturesWindow.CreateButtons();
        creaturesWindow.Refresh();
    }

    void Update()
    {
        moneyIndicator.text = AlteredStringForm(money) + "$";
        speedIndicator.text = AlteredStringForm(incomeSpeed) + "$/s";
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

    public void ShowWindow(ShopWindowController window)
    {
        CreatureController.canCreturesBeMoved = false;
        window.gameObject.SetActive(true);
    }

    public void HideWindow(ShopWindowController window)
    {
        CreatureController.canCreturesBeMoved = true;
        window.gameObject.SetActive(false);
    }


    public void CreateCreatureProducts()
    {
        Sprite[] creatureSprites = Settings.main.spritesByLevel;
        creaturesWindow.buyables = new Buyable[creatureSprites.Length];

        for (int lv = 0; lv < creatureSprites.Length; lv++)
        {
            Buyable prod = new BuyableCreature
            {
                sprite = creatureSprites[lv],
                name = "Creature " + lv,
                creatureLevel = lv,
                description = "Buy a level " + lv + " creature.",
                initialPrice = 1 + lv,
                priceAdd = 1 + lv,
                priceMultiplier = 2M
            };
            creaturesWindow.buyables[lv] = prod;
        }

    }





}
