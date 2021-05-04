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
    public List<CreatureData> creatures = new List<CreatureData>();

    private decimal price1 = 1, price = 0;
    public int maxCreatureLevel, unlockedCreatures;

    private int[] creatureButtonsGrades;

    [Header("Bonuses")]
    public decimal globalBonus;

    private void Awake()
    {
        main = this;
        settings = Settings.main;
        saveManager = GetComponent<SaveManager>();
    }

    void Start()
    {
        ui = UIController.main;

        bool isGameLoaded = LoadGame();
        if(!isGameLoaded)
        {
            settings.MakeSpritesArray();
            settings.MakeNewCreatureNames();
            creatureButtonsGrades = new int[settings.creatureSpeciesCount];
        }

        CreateCreatureProducts();

        ui.creaturesWindow.CreateButtons();
        ui.creaturesWindow.SetButtonsGrades(creatureButtonsGrades);
        ui.creaturesWindow.Refresh();

        RecalculateIncomeSpeedAndMaxLevels();
        unlockedCreatures = maxCreatureLevel;
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

        var creature = creatureObj.GetComponent<CreatureData>();
        creatures.Add(creature);
        creature.level = lv;
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
            + globalBonus);
    }

    public decimal GetIncomeSpeed(int creatureLevel)
    {
        return Money.DecimalPow(creatureLevel, 2);
    }

    public void RecalculateIncomeSpeedAndMaxLevels()
    {
        incomeSpeed = 0;
        foreach (var creature in creatures)
        { 
            incomeSpeed += GetIncomeSpeed(creature.level);
            maxCreatureLevel = Mathf.Max(maxCreatureLevel, creature.level);
        }
        ui.creaturesWindow.unlockedButtons = settings.buyableLevelsUnlocked[maxCreatureLevel] + 1;
    }

    public void SaveGame()
    {
        SaveData save = new SaveData();
        save.spritesOrder = settings.GetSpritesOrder();
        save.creatureNames = settings.namesByLevel;
        save.money = money;

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
        save.creatureShopGrades = ui.creaturesWindow.GetButtonsGrades();

        saveManager.WriteSaveFile(save);
    }

    public bool LoadGame()
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

            creatureButtonsGrades = save.creatureShopGrades;

            return true;
        }
        return false;
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
        Sprite[] creatureSprites = Settings.main.spritesByLevel;
        ui.creaturesWindow.buyables = new Buyable[settings.creatureSpeciesCount];

        for (int lv = 0; lv < settings.creatureSpeciesCount; lv++)
        {
            Buyable prod = new BuyableCreature()
            {
                creatureLevel = lv
            };

            prod.sprite = creatureSprites[lv];

            prod.name = settings.namesByLevel[lv];
            prod.description = prod.name + " gives you " + 
                UIController.AlteredStringForm(GetIncomeSpeed(lv)) + Money.symbol + 
                "/s and " + UIController.AlteredStringForm(GetIncome(lv)) + Money.symbol +
                " for a click.";

            prod.initialPrice = Money.DecimalPow(2, lv) + lv;
            prod.priceAdd = 1 + lv;
            prod.priceMultiplier = 1.2M;

            ui.creaturesWindow.buyables[lv] = prod;
        }
    }

    public void CheckCongratulations(int level)
    {
        if (level > unlockedCreatures)
        {
            Congratulate(level);
            unlockedCreatures = level;
        }
    }


    void Congratulate(int level)
    {
        ui.congratsWindow.creatureLevel = level;
        ui.ShowWindow(ui.congratsWindow);
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
        if(pause)
            SaveGame();
    }

}
