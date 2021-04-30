using UnityEngine;

[System.Serializable]
public class Buyable
{
    public Sprite sprite;
    public string name;
    public string description;

    public decimal initialPrice;
    public decimal priceMultiplier;
    public decimal priceAdd;
}

[System.Serializable]
public class BuyableCreature : Buyable
{
    public int creatureLevel;
}

[System.Serializable]
public class BuyableUpgrade : Buyable
{
    public int upgradedCreatureLevel;
}

