using UnityEngine;

[CreateAssetMenu(fileName = "New Creatures", menuName = "Creature")]
public class CreatureTemplate : ScriptableObject
{
    [SerializeField]
    private int level;
    [SerializeField]
    private Sprite sprite;

    [SerializeField, Range(0, 1), Tooltip("0-only auto-income, 1-only click income")]
    private float incomeModifier = 0.5f;

    [SerializeField]
    private CreatureTemplate[] nextForms = new CreatureTemplate[2];
}
