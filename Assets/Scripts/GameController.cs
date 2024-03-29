﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public event System.Action<int> OnCretureUnlocked;
    public event System.Action<decimal, Vector3> OnMoneyChanged;

    public static GameController main;    

    [Header("To Link")]
    [SerializeField]
    private GameObject creaturePrefab;
    [SerializeField]
    private Transform creaturesContainer;

    public string saveName;

    [SerializeField]
    private Settings settings;
    public Settings Settings => settings;

    [Header("States")]
    [SerializeField]
    private decimal money;
    public decimal CurrentMoney => money;

    [SerializeField]
    private decimal incomeSpeed;
    public decimal IncomeSpeed => incomeSpeed;

    [SerializeField]
    private List<CreatureData> creatures = new List<CreatureData>();
    public IReadOnlyList<CreatureData> Creatures => creatures;

    [SerializeField]
    private int maxCreatureLevel;
    [SerializeField]
    private int unlockedCreatures;

    private int[] creatureButtonsGrades;
    public IReadOnlyList<int> CreatureButtonsGrades => creatureButtonsGrades;

    [Header("Bonuses")]
    [SerializeField]
    private decimal globalBonus;

    private void Awake()
    {
        main = this;
    }

    void Start()
    {
        bool isGameLoaded = LoadGame();
        if(!isGameLoaded)
        {
            settings.MakeSpritesArray();
            settings.MakeNewCreatureNames();
            creatureButtonsGrades = new int[settings.CreatureSpeciesCount];
        }

        RecalculateIncomeSpeedAndMaxLevels();
        unlockedCreatures = maxCreatureLevel;
        //StartCoroutine(nameof(RegularSaveGameCo));
        StartCoroutine(nameof(UpdateIncomeSpeed));
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

    public void RemoveCreature(CreatureData creature)
    {
        creatures.Remove(creature);
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


    public void CreateCreature(int level)
    {
        GameObject creatureObj = Instantiate(creaturePrefab, creaturesContainer);
        creatureObj.transform.position = new Vector3(
            Random.Range(settings.CreaturesArea.xMin, settings.CreaturesArea.xMax),
            Random.Range(settings.CreaturesArea.yMin, settings.CreaturesArea.yMax));
        creatureObj.name = "Creature " + level;

        var creature = creatureObj.GetComponent<CreatureData>();
        creature.Init(level);

        creatures.Add(creature);
        maxCreatureLevel = Mathf.Max(maxCreatureLevel, level);
    }

    public void AddMoneyFromCreature(CreatureData creature)
    {
        decimal income = GetIncome(creature.Level);
        money += income;
        OnMoneyChanged?.Invoke(income, creature.transform.position);
    }

    public  decimal GetIncome(int creatureLevel)
    {
        return Money.DecimalPow(2, creatureLevel) + creatureLevel + globalBonus;
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
            incomeSpeed += GetIncomeSpeed(creature.Level);
            maxCreatureLevel = Mathf.Max(maxCreatureLevel, creature.Level);
        }
        //ui.creaturesWindow.unlockedButtons = settings.buyableLevelsUnlocked[maxCreatureLevel] + 1;
    }

    public void SaveGame()
    {
        return;
        SaveData save = new SaveData();
        save.spritesOrder = settings.GetSpritesOrder();
        //save.creatureNames = settings.namesByLevel;
        save.money = money;

        int maxLevel = 0;
        foreach(var creature in creatures)
        {
            if (creature.Level > maxLevel)
                maxLevel = creature.Level;
        }

        int[] creaturesCounts = new int[maxLevel + 1];
        foreach (var creature in creatures)
            creaturesCounts[creature.Level]++;
        save.creatures = creaturesCounts;
        //save.creatureShopGrades = ui.creaturesWindow.GetButtonsGrades();

        SaveManager.WriteSaveFile(save, saveName);
    }

    public bool LoadGame()
    {
        SaveData save = SaveManager.ReadSaveFile<SaveData>(saveName);
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
            {
                //settings.namesByLevel = save.creatureNames;
            }
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

    public void UnlockCreature(int level)
    {
        if (level > unlockedCreatures)
        {
            unlockedCreatures = level;
            OnCretureUnlocked?.Invoke(level);
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
        if(pause)
            SaveGame();
    }

}
