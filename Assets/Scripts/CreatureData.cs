using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureData : MonoBehaviour
{
    public static AnimationCurve scaleCurve;
    public static int maxLevel;

    private Settings settings;

    [Header("To Link")]
    public new SpriteRenderer renderer;
    [HideInInspector]
    public new Collider2D collider;

    [Header("States")]
    public int level; // from 0 to maxLevel

    private void Awake()
    {
        collider = GetComponent<Collider2D>();
    }


    void Start()
    {
        settings = Settings.main;
        Refresh();
    }

    void Update()
    {

    }

    void Refresh()
    {
        transform.localScale = Vector3.one * scaleCurve.Evaluate(1f * level / maxLevel);
        renderer.sprite = settings.spritesByLevel[level];
    }

    public void LevelUp()
    {
        level += 1;
        Refresh();
    }
}
