using UnityEngine;

[CreateAssetMenu(fileName = "New Creatures", menuName = "Creature")]
public class CreatureTemplate : ScriptableObject
{
    public int level;
    public Sprite sprite;

    [Range(0,1)]
    [Header("0-only auto-income, 1-only click income")]
    public float incomeModifier = 0.5f;

    public CreatureTemplate[] nextForms = new CreatureTemplate[2];

}