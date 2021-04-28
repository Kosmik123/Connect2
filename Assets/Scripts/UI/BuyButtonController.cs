using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class BuyButtonController : MonoBehaviour
{
    [Header("To Link")]
    public Text nameText;
    public Text descriptionText;
    public Image graphicImage;

    [Header("Settings")]
    public Buyable product;

    private void Awake()
    {
    }


    public void Refresh()
    {
        nameText.text = product.name + " (" +
            UIController.AlteredStringForm(product.price) + ")";
        descriptionText.text = product.description;
        graphicImage.sprite = product.sprite;
    }


}
