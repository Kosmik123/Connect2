using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class BuyButtonController : MonoBehaviour
{
    [Header("To Link")]
    public Text nameText;
    public Text priceText;
    public Text descriptionText;
    public Image graphicImage;
    private GameController gameController;

    [Header("Settings")]
    public Buyable product;

    [Header("States")]
    public int grade = 0;
    public decimal price;


    private void Start()
    {
        gameController = GameController.main;

    }


    public void Refresh()
    {
        price = decimal.Round(Money.DecimalPow(product.priceMultiplier, grade) *
            (product.initialPrice + grade * product.priceAdd) );
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
            grade++;
            Refresh();
            gameController.RecalculateIncomeSpeedAndMaxLevels();
        }
    }

}
