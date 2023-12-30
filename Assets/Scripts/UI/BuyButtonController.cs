using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class BuyButtonController : MonoBehaviour
{
    [Header("To Link")]
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Text priceText;
    [SerializeField]
    private Text descriptionText;
    [SerializeField]
    private Image graphicImage;
    private GameController gameController;

    [Header("Settings")]
    [SerializeField]
    private Buyable product;
    public Buyable Product
    {
        get => product;
        set => product = value;
    }

    [Header("States")]
    [SerializeField]
    private int grade = 0;
    public int Grade 
    { 
        get => grade; 
        set => grade = value; 
    }

    [SerializeField]
    private decimal price;


    private void Start()
    {
        gameController = GameController.main;
    }

    public void Refresh()
    {
        price = decimal.Round(Money.DecimalPow(product.priceMultiplier, Grade) *
            (product.initialPrice + Grade * product.priceAdd) );
        priceText.text = UIController.AlteredStringForm(price) + Money.symbol;

        nameText.text = product.name;
        descriptionText.text = product.description;
        graphicImage.sprite = product.sprite;
    }

    public void Buy()
    {
        bool success = gameController.BuyProduct(product, price);
        if(success)
        {
            Grade++;
            Refresh();
            gameController.RecalculateIncomeSpeedAndMaxLevels();
        }
    }

}
