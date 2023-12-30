using System;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("To Link")]
    [SerializeField]
    private Transform worldCanvas;
    [SerializeField]
    private GameObject incomeIndicatorPrefab;

    [SerializeField]
    private Text moneyIndicator;
    [SerializeField]
    private Text speedIndicator;
    //public Text priceIndicator;

    [SerializeField]
    private ShopWindowController upgradesWindow;
    [SerializeField]
    private ShopWindowController creaturesWindow;
    [SerializeField]
    private CongratsWindow congratsWindow;

    [Header("Properties")]
    [SerializeField]
    private decimal money, incomeSpeed;
    [SerializeField]
    private decimal price;

    private GameController game;

    private void OnEnable()
    {
        game = GameController.main;
        if (game)
        {
            SubscribeGameEvents();
        }
    }

    private void SubscribeGameEvents()
    {
        game.OnCretureUnlocked += Congratulate;
        game.OnMoneyChanged += ShowIncome;
    }


    void Start()
    {
        if (game == null)
        {
            game = GameController.main;
            SubscribeGameEvents();
        }

        CreateCreatureProducts();
        creaturesWindow.CreateButtons();
        creaturesWindow.SetButtonsGrades(game.CreatureButtonsGrades);
        creaturesWindow.Refresh();
    }


    void Update()
    {
        moneyIndicator.text = AlteredStringForm(money) + Money.symbol;
        speedIndicator.text = AlteredStringForm(incomeSpeed) + Money.symbol + "/s";

        if (congratsWindow.gameObject.activeInHierarchy)
            if (Input.GetMouseButton(0))
                HideWindow(congratsWindow);

        money = game.money;
        incomeSpeed = game.incomeSpeed;
    }

    private void ShowIncome(decimal income, Vector3 position)
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

    private void CreateCreatureProducts()
    {
        Sprite[] creatureSprites = game.Settings.spritesByLevel;
        creaturesWindow.Init(game.Settings.creatureSpeciesCount);
        for (int level = 0; level < game.Settings.creatureSpeciesCount; level++)
        {
            var product = new BuyableCreature(level)
            {
                sprite = creatureSprites[level],
                name = game.Settings.namesByLevel[level],

                initialPrice = decimal.Round(Money.DecimalPow(2.2M, level) + level),
                priceAdd = 1 + level,
                priceMultiplier = 1.2M
            };
            product.description = product.name + " gives you " +
                AlteredStringForm(game.GetIncomeSpeed(level)) + Money.symbol +
                "/s and " + AlteredStringForm(game.GetIncome(level)) + Money.symbol +
                " for a click.";

            creaturesWindow.SetProduct(level, product);
        }
    }

    private void Congratulate(int level)
    {
        congratsWindow.CreatureLevel = level;
        ShowWindow(congratsWindow);
        creaturesWindow.UnlockedButtons += 1;
        creaturesWindow.Refresh();
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

    private void OnDisable()
    {
        game.OnCretureUnlocked -= Congratulate;
        game.OnMoneyChanged -= ShowIncome;
    }
}
