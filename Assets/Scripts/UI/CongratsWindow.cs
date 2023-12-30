using UnityEngine;
using UnityEngine.UI;

public class CongratsWindow : Window
{
    [Header("To Link")]
    [SerializeField]
    private Text creatureName;
    [SerializeField]
    private Image creatureSprite;

    [Header("Properties")]
    [SerializeField]
    private int creatureLevel;
    public int CreatureLevel
    {
        get => creatureLevel;
        set => creatureLevel = value;
    }

    public override void Refresh()
    {
        var settings = GameController.main.Settings;
        creatureName.text = settings.GetCreatureName(creatureLevel);
        creatureSprite.sprite = settings.GetCreatureSprite(creatureLevel);
    }
}
