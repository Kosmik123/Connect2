using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings main;
    [SerializeField]
    private CreaturesSettings settings;

    [Header("Creatures")]
    public Rect creaturesArea;
    public int maxLevel;
    public AnimationCurve scaleByLevel;

    public string textureName;
    public Sprite[] spritesByLevel;

    private void Awake()
    {
        main = this;
        MakeRandomSpriteArray();
    }

    private void Start()
    {
        CreatureData.scaleCurve = scaleByLevel;
        CreatureData.maxLevel = maxLevel;        
        
        
        CreatureController.movementRange = creaturesArea;

    
    }

    void MakeRandomSpriteArray()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(textureName);
        spritesByLevel = new Sprite[sprites.Length];

        List<int> indexes = new List<int>();
        for(int i = 0; i < sprites.Length; i++)
        {
            indexes.Add(i);
        }

        for(int i = 0; i < spritesByLevel.Length; i++)
        {
            int index = Random.Range(0, indexes.Count);
            int sprIndex = indexes[index];
            indexes.RemoveAt(index);
            spritesByLevel[i] = sprites[sprIndex];
        }
    }



#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(creaturesArea.center, creaturesArea.size);
    }

#endif

}
