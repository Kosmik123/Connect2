using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopWindowController : MonoBehaviour
{

    [Header("To Link")]
    public RectTransform buttonsContainer;

    [Header("Prefabs")]
    public GameObject buttonPrefab;

    [Header("Properties")]
    public float startingHeight = -200;
    public float buttonsDistance = -350;

    [Header("States")]
    public int unlockedButtons = 0;
    public BuyButtonController[] buttons;
    public Buyable[] buyables;


    public void CreateButtons()
    {
        buttons = new BuyButtonController[buyables.Length];
        for(int lv = 0; lv < buyables.Length; lv++)
        {
            CreateNewButton(lv, buyables[lv]);
        }
    }

    public void Refresh()
    {
        buttonsContainer.sizeDelta = new Vector2(
            buttonsContainer.sizeDelta.x,
            unlockedButtons * Mathf.Abs(buttonsDistance) + Mathf.Abs(startingHeight));
        buttonsContainer.localPosition = -buttonsContainer.sizeDelta.y * Vector3.down;

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].transform.localPosition = new Vector3(0, buttonsContainer.rect.yMax + startingHeight + i * buttonsDistance, 0);
            buttons[i].gameObject.SetActive(i < unlockedButtons);
        }
    }

    private void CreateNewButton(int buttonIndex, Buyable product)
    {
        var button = Instantiate(buttonPrefab, buttonsContainer);
        button.name = "Product Button " + buttonIndex;
        BuyButtonController buyButton = button.GetComponent<BuyButtonController>();
        buyButton.product = product;
        buyButton.transform.localPosition = new Vector3(0, startingHeight + buttonIndex * buttonsDistance, 0);
        buttons[buttonIndex] = buyButton;
        buyButton.Refresh();
    }

}
