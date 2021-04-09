using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController main;


    [Header("To Link")]
    public Transform worldCanvas;
    public GameObject incomeIndicatorPrefab;
    public Text moneyIndicator;

    public long money;


    private void Awake()
    {
        main = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moneyIndicator.text = money.ToString();
    }

    public void ShowIncome(long amount, Vector3 position)
    {
        var indicator = Instantiate(incomeIndicatorPrefab, worldCanvas);
        indicator.transform.position = position;
        indicator.GetComponentInChildren<Text>().text = "+" + amount.ToString();
    }

}
