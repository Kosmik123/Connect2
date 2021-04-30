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
        RecalculateIncomeSpeed();

        creatureMultipliers = new decimal[settings.spritesByLevel.Length];
        for (int i = 0; i < creatureMultipliers.Length; i++)
            creatureMultipliers[i] = 1;

    }

    void Start()
    {
        ui = UIController.main;
        StartCoroutine("RegularSaveGameCo");
        StartCoroutine("UpdateIncomeSpeed");
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
            if (incomeSpeed < 100 && incomeSpeed > 0)
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
            Debug.Log("Level kupionego to: " + buyable.creatureLevel);
            CreateCreature(buyable.creatureLevel);
        }
        else
        {
            Debug.Log("Kupiono bonus");
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
    }

    public void AddMoneyFromCreature(CreatureData creature)
    {
        decimal income = GetIncome(creature.level);
        ui.ShowIncome(income, creature.transform.position);
        money += (income);
    }

    private decimal GetIncome(int creatureLevel)
    {
        return (Money.DecimalPow(2, creatureLevel) + creatureLevel
            + globalBonus) * creatureMultipliers[creatureLevel];
    }

    private decimal GetIncomeSpeed(int creatureLevel)
    {
        return Money.DecimalPow(2, creatureLevel) - 1;
    }

    public void RecalculateIncomeSpeed()
    {
        incomeSpeed = 0;
        creatures = creaturesContainer.GetComponentsInChildren<CreatureData>();
        foreach (var creature in creatures)
            incomeSpeed += GetIncomeSpeed(creature.level);
    }

    public void SaveGame()
    {
        SaveData save = new SaveData();
        save.spritesOrder = settings.GetSpritesOrder();
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
        }
        else
        {
            settings.MakeSpritesArray();
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
}
