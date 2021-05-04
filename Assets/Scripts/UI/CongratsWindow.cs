using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CongratsWindow : Window
{
    public Settings settings;

    [Header("To Link")]
    public Text creatureName;
    public Image creatureSprite;

    [Header("Properties")]
    public int creatureLevel;

    private void Start()
    {
        settings = Settings.main;
    }

    public override void Refresh()
    {
        settings = Settings.main;
        creatureName.text = settings.namesByLevel[creatureLevel];
        creatureSprite.sprite = settings.spritesByLevel[creatureLevel];
    }
}
