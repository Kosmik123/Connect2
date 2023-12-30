using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Window : MonoBehaviour
{
    public virtual void Refresh()
    {

    }
}

public class ShopWindowController : Window
{

    [Header("To Link")]
    public  GridLayoutGroup buttonsContainer;

    [Header("Prefabs")]
    public GameObject buttonPrefab;

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

    public override void Refresh()
    {
        RectTransform buttonsContainerTransform = (RectTransform)buttonsContainer.transform;
        buttonsContainerTransform.sizeDelta = new Vector2(
            buttonsContainerTransform.sizeDelta.x,
            unlockedButtons * (buttonsContainer.cellSize.y + buttonsContainer.spacing.y));
        buttonsContainer.transform.localPosition = -buttonsContainerTransform.sizeDelta.y * Vector3.down;

        for (int i = 0; i < buttons.Length; i++)
        {
            //buttons[i].transform.localPosition = new Vector3(0, buttonsContainer.rect.yMax + startingHeight + i * buttonsDistance, 0);
            buttons[i].gameObject.SetActive(i < unlockedButtons);
            buttons[i].Refresh();
        }
        Debug.Log("New scroll view height = {$buttonsContainerTransform.sizeDelta.y}");
    }

    private void CreateNewButton(int buttonIndex, Buyable product)
    {
        var button = Instantiate(buttonPrefab, buttonsContainer.transform);
        button.name = "Product Button " + buttonIndex;
        BuyButtonController buyButton = button.GetComponent<BuyButtonController>();
        buyButton.product = product;
        //buyButton.transform.localPosition = new Vector3(0, startingHeight + buttonIndex * buttonsDistance, 0);
        buttons[buttonIndex] = buyButton;
        buyButton.Refresh();
    }

    public int[] GetButtonsGrades()
    {
        int[] grades = new int[buttons.Length];
        for(int i = 0; i < buttons.Length; i++)
            grades[i] = buttons[i].grade;

        return grades;
    }

    public void SetButtonsGrades(int[] grades)
    {
        int len = Mathf.Min(grades.Length, buttons.Length);
        for (int i = 0; i < len; i++)
            buttons[i].grade = grades[i];
    }


}
