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
    private string savefileName;

    [Header("States")]
    public decimal money, incomeSpeed;
    public CreatureData[] creatures;
    public string saveFile = "";
    private decimal price1 = 1, price = 0;

    [Header("Bonuses")]
    public decimal globalBonus;
    public decimal[] creatureMultipliers;

    private void Awake()
    {
        main = this;
        saveManager = GetComponent<SaveManager>();
    }

    void Start()
    {
        ui = UIController.main;
        settings = Settings.main;

        LoadGame();
        StartCoroutine("RegularSaveGameCo");
        StartCoroutine("UpdateIncomeSpeed");

        creatureMultipliers = new decimal[settings.spritesByLevel.Length];
        for(int i = 0; i < creatureMultipliers.Length; i++)
            creatureMultipliers[i] = 1;
    }

    void Update()
    {
        ui.money = money;
        ui.price = price;
    }

    IEnumerator UpdateIncomeSpeed()
    {
        while(true)
        {
            decimal lastMoney = money;
            yield return new WaitForSeconds(1);
            incomeSpeed = money - lastMoney;
            ui.incomeSpeed = incomeSpeed;
        }
    }


    public void BuyCreature()
    {
        if (money < price)
            return;

        CreateCreature();
        money -= price;

        decimal newPrice = price + price1;
        price1 = price;
        price = newPrice;
    }

    public void CreateCreature(int lv = 0)
    {
        GameObject creatureObj = Instantiate(creaturePrefab, creaturesContainer);
        creatureObj.transform.position = new Vector3(
            Random.Range(settings.creaturesArea.xMin, settings.creaturesArea.xMax),
            Random.Range(settings.creaturesArea.yMin, settings.creaturesArea.yMax));
        creatureObj.name = "Creature " + lv;
        creatureObj.GetComponent<CreatureData>().level = lv;
    }

    public void AddMoney(CreatureData creature)
    {
        decimal income = GetIncome(creature.level);
        ui.ShowIncome(income, creature.transform.position);
        money += (income);
    }

    private decimal GetIncome(int creatureLevel)
    {
        return (Mathf.RoundToInt(Mathf.Pow(2, 2*creatureLevel)) + creatureLevel
            + globalBonus) * creatureMultipliers[creatureLevel];
    }

    public void SaveGame()
    {
        string save = "";
        save += money.ToString() + "\n";
        creatures = creaturesContainer.GetComponentsInChildren<CreatureData>();
        foreach (var creature in creatures)
            save += creature.level + ",";
        saveFile = save.Substring(0, save.Length-1);

        saveManager.WriteSaveFile(saveFile);
    }

    public void LoadGame()
    {
        CreatureData[] creatures = creaturesContainer.GetComponentsInChildren<CreatureData>();
        foreach (var creature in creatures)
            Destroy(creature.gameObject);

        saveFile = saveManager.ReadSaveFile();
        if (saveFile.Contains("\n"))
        {
            string[] parts = saveFile.Split('\n');
            money = DecimalParseFast(parts[0]);
            string[] levels = parts[1].Split(',');


            foreach (var lv in levels)
            {
                int level = IntParseFast(lv);
                CreateCreature(level);
            }
        }
    }

    public static int IntParseFast(string value)
    {
        int result = 0;
        for (int i = 0; i < value.Length; i++)
        {
            char letter = value[i];
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
            result = (10 * result) + (letter - 48);
        }
        return result;
    }

    private IEnumerator RegularSaveGameCo()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            Debug.Log("Zapisywanie");
            SaveGame();
        }
    }
}
