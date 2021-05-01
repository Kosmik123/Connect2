using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour
{
    public static GameController main;    

    [Header("To Link")]
    public GameObject creaturePrefab;
    public Transform creaturesContainer;

    private SaveManager saveManager;
    private UIController ui;
    private Settings settings;

    [Header("States")]
    public decimal money, incomeSpeed;
    public CreatureData[] creatures;

    private decimal price1 = 1, price = 0;
    public int maxCreatureLevel;

    [Header("Bonuses")]
    public decimal globalBonus;
    public decimal[] creatureMultipliers;

    private void Awake()
    {
        main = this;
        settings = Settings.main;
        settings.MakeSpritesArray();

        saveManager = GetComponent<SaveManager>();

        LoadGame();

        settings.buyableLevelsUnlocked = new int[settings.creaturesCount];
        creatureMultipliers = new decimal[settings.creaturesCount];
        for (int lv = 0; lv < settings.creaturesCount; lv++)
        {
            settings.buyableLevelsUnlocked[lv] = lv / 2;
            creatureMultipliers[lv] = 1;
        }
        

    }

    void Start()
    {
        ui = UIController.main;

        RecalculateIncomeSpeedAndMaxLevels();
        CreateCreatureProducts();
        ui.creaturesWindow.CreateButtons();
        ui.creaturesWindow.Refresh();

        //StartCoroutine(nameof(RegularSaveGameCo));
        StartCoroutine(nameof(UpdateIncomeSpeed));
    }

    void Update()
    {
        ui.money = money;
        ui.price = price;
        ui.incomeSpeed = incomeSpeed;
    }

    IEnumerator UpdateIncomeSpeed()
    {
        while(true)
        {
            if (incomeSpeed < 1000 && incomeSpeed > 0)
            {
                yield return new WaitForSeconds((float) (1 / incomeSpeed));
                money += 1;
            }
            else 
            {
                yield return new WaitForSeconds(0.1f);
                money += incomeSpeed / 10;
            }
        }
    }

    public bool BuyProduct(Buyable product, decimal price)
    {
        if (money < price)
            return false;

        money -= price;
        
        if(product.GetType() == typeof(BuyableCreature))
        {
            BuyableCreature buyable = (BuyableCreature)product;
            Debug.Log("Buying creature with level: " + buyable.creatureLevel);
            CreateCreature(buyable.creatureLevel);
        }
        else
        {
            Debug.Log("Buying bonus");
        }
        return true;
    }


    public void CreateCreature(int lv)
    {
        GameObject creatureObj = Instantiate(creaturePrefab, creaturesContainer);
        creatureObj.transform.position = new Vector3(
            Random.Range(settings.creaturesArea.xMin, settings.creaturesArea.xMax),
            Random.Range(settings.creaturesArea.yMin, settings.creaturesArea.yMax));
        creatureObj.name = "Creature " + lv;
        creatureObj.GetComponent<CreatureData>().level = lv;
        maxCreatureLevel = Mathf.Max(maxCreatureLevel, lv);
    }

    public void AddMoneyFromCreature(CreatureData creature)
    {
        decimal income = GetIncome(creature.level);
        ui.ShowIncome(income, creature.transform.position);
        money += (income);
    }

    public  decimal GetIncome(int creatureLevel)
    {
        return (Money.DecimalPow(2, creatureLevel) + creatureLevel
            + globalBonus) * creatureMultipliers[creatureLevel];
    }

    public decimal GetIncomeSpeed(int creatureLevel)
    {
        return Money.DecimalPow(2, creatureLevel) - 1;
    }

    public void RecalculateIncomeSpeedAndMaxLevels()
    {
        incomeSpeed = 0;
        creatures = creaturesContainer.GetComponentsInChildren<CreatureData>();
        foreach (var creature in creatures)
        { 
            incomeSpeed += GetIncomeSpeed(creature.level);
            maxCreatureLevel = Mathf.Max(maxCreatureLevel, creature.level);
        }
        ui.creaturesWindow.unlockedButtons = settings.buyableLevelsUnlocked[maxCreatureLevel];
    }

    public void SaveGame()
    {
        SaveData save = new SaveData();
        save.spritesOrder = settings.GetSpritesOrder();
        save.creatureNames = settings.namesByLevel;
        save.money = money;

        creatures = creaturesContainer.GetComponentsInChildren<CreatureData>();
        int maxLevel = 0;
        foreach(var creature in creatures)
        {
            if (creature.level > maxLevel)
                maxLevel = creature.level;
        }

        int[] creaturesCounts = new int[maxLevel + 1];
        foreach (var creature in creatures)
            creaturesCounts[creature.level]++;
        save.creatures = creaturesCounts;

        saveManager.WriteSaveFile(save);
    }

    public void LoadGame()
    {
        SaveData save = saveManager.ReadSaveFile();
        if (save != null)
        {
            money = save.money;

            CreatureData[] creatures = creaturesContainer.GetComponentsInChildren<CreatureData>();
            if (creatures != null) foreach (var creature in creatures)
                    Destroy(creature.gameObject);

            int unlockedLevels = save.creatures.Length;
            for (int lv = 0; lv < unlockedLevels; lv++)
            {
                int creaturesCount = save.creatures[lv];
                for (int i = 0; i < creaturesCount; i++)
                {
                    CreateCreature(lv);
                }
            }
            settings.MakeSpritesArray(save.spritesOrder);
            if (save.creatureNames == null || save.creatureNames.Length != save.spritesOrder.Length)
                settings.MakeNewCreatureNames();
            else
                settings.namesByLevel = save.creatureNames;
        }
        else
        {
            settings.MakeSpritesArray();
            settings.MakeNewCreatureNames();
        }
    }

    public static int IntParseFast(string value)
    {
        int result = 0;
        for (int i = 0; i < value.Length; i++)
        {
            char letter = value[i];
            if (letter < 48 || letter > 57)
                break;
            result = 10 * result + (letter - 48);
        }
        return result;
    }

    public static decimal DecimalParseFast(string value)
    {
        decimal result = 0;
        for (int i = 0; i < value.Length; i++)
        {
            char letter = value[i];
            if (letter < 48 || letter > 57)
                break;
            result = (10 * result) + (letter - 48);
        }
        return result;
    }

    public void CreateCreatureProducts()
    {
        GameController gc = GameController.main;

        Sprite[] creatureSprites = Settings.main.spritesByLevel;
        ui.creaturesWindow.buyables = new Buyable[settings.creaturesCount];

        for (int lv = 0; lv < settings.creaturesCount; lv++)
        {
            Buyable prod = new BuyableCreature()
            {
                creatureLevel = lv
            };

            prod.sprite = creatureSprites[lv];

            prod.name = settings.namesByLevel[lv][0].ToString().ToUpper() +
                settings.namesByLevel[lv].Substring(1);
            prod.description = prod.name + " gives you " + 
                UIController.AlteredStringForm(gc.GetIncomeSpeed(lv)) + Money.symbol + 
                "/s and " + UIController.AlteredStringForm(gc.GetIncome(lv)) + Money.symbol +
                " for a click.";

            prod.initialPrice = Money.DecimalPow(2, lv);
            prod.priceAdd = 1 + lv;
            prod.priceMultiplier = 1.2M;

            ui.creaturesWindow.buyables[lv] = prod;
        }
    }


    private IEnumerator RegularSaveGameCo()
    {
        while (true)
        {
            yield return new WaitForSeconds(10);
            Debug.Log("Zapisywanie");
            SaveGame();
        }
    }

    void OnApplicationQuit()
    {
        SaveGame();
        Debug.Log("Application ending after " + Time.time + " seconds");
    }

    private void OnApplicationPause(bool pause)
    {
        SaveGame();
    }

}
