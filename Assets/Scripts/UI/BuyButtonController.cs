using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buyable
{
    public Sprite sprite;
    public string name;
    public string description;
    public decimal price;
    public int grade;
}

public class BuyableCreature : Buyable
{
    public int creatureLevel;
}

public class BuyableUpgrade : Buyable
{
    public int upgradedCreatureLevel;
}



public class BuyButtonController : MonoBehaviour
{



    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
