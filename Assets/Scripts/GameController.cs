using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public class Money
    {
        public static char[] prefixes = {'k', 'M', 'G', 'T', 'P', 'E', 'Z', 'Y'};
        public const int HIGH_NUMBER = 1000000000;

        public int[] units = new int[3];

        public Money()
        {
        }


        public string ToString()
        {
            string result = "";
            string prefix = "";
            int highestNumber;

            int prefixIndexReversed = 1;
            for (int j = units.Length-1; j >= 0; j--)
            {
                highestNumber = HIGH_NUMBER;
                for (int i = 0; i < 3; i++)
                { 
                    if (units[j] >= highestNumber)
                    {
                        
                    }
                            
                            
                }



            }




            return result;
        }

        public void Add()
        {

            Calculate();
        }

        private void Calculate()
        {

        }
    }

    public static GameController main;    

    [Header("To Link")]
    public GameObject creaturePrefab;
    public Transform creaturesContainer;
    
    private UIController ui;
    private Settings settings;
    private string savefileName;

    [Header("States")]
    public long money;
    public CreatureData[] creatures;
    public string saveFile = "";

    // Price
    long price1, price = 1;


    private void Awake()
    {
        main = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        ui = UIController.main;
        settings = Settings.main;

        string filepath = Application.persistentDataPath + "/" + settings.savefileName;
        if (File.Exists(filepath))
            LoadGame();
    }

    // Update is called once per frame
    void Update()
    {
        ui.money = money;
        ui.price = price;
    }


    public void BuyCreature()
    {
        if (money < price)
            return;

        CreateCreature();
        money -= price;

        long newPrice = price + price1;
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
        long income = Mathf.RoundToInt(Mathf.Pow(2, creature.level+1)) - creature.level ;
        ui.ShowIncome(income, creature.transform.position);
        money += income;
    }

    public void SaveGame()
    {
        string save = "";
        creatures = creaturesContainer.GetComponentsInChildren<CreatureData>();
        foreach (var creature in creatures)
            save += creature.level + ",";
        saveFile = save.Substring(0, save.Length-1);

        WriteSaveFile(saveFile);
    }

    public void WriteSaveFile(string contents)
    {
        string filepath = Application.persistentDataPath + "/" + settings.savefileName;
        File.WriteAllText(filepath, contents);        
    }

    public void LoadGame()
    {
        CreatureData[] creatures = creaturesContainer.GetComponentsInChildren<CreatureData>();
        foreach (var creature in creatures)
            Destroy(creature.gameObject);

        saveFile = ReadSaveFile();

        string[] levels = saveFile.Split(',');
        foreach (var lv in levels)
        {
            int level = IntParseFast(lv);
            CreateCreature(level);
        }
    }

    public string ReadSaveFile()
    {
        string filepath = Application.persistentDataPath + "/" + settings.savefileName;
        return File.ReadAllText(filepath);
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

    private IEnumerator RegularSaveGame()
    {
        while (true)
        {
            yield return new WaitForSeconds(10);
            SaveGame();
        }
    }

}
