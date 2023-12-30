using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField]
    private CreaturesSettings settings;
    [SerializeField]
    private RandomNamesGenerator randomNamesGenerator;

    [Header("Creatures")]
    [SerializeField]
    private int creatureSpeciesCount;
    public int CreatureSpeciesCount => creatureSpeciesCount;

    [SerializeField]
    private Rect creaturesArea;
    public Rect CreaturesArea => creaturesArea;

    [SerializeField]
    private int maxLevel;
    [SerializeField]
    private AnimationCurve scaleByLevel;

    [SerializeField]
    private string textureName;
    [SerializeField]
    private Sprite[] spritesByLevel;
    [SerializeField]
    private string[] namesByLevel;
    [SerializeField]
    private int[] buyableLevelsUnlocked;

    private Sprite[] sprites;

    private void Awake()
    {
        sprites = Resources.LoadAll<Sprite>(textureName);
        creatureSpeciesCount = sprites.Length;
    }

    private void Start()
    {
        CreatureData.scaleCurve = scaleByLevel;
        CreatureData.maxLevel = maxLevel;        
        
        CreatureController.movementRange = CreaturesArea;

        buyableLevelsUnlocked = new int[creatureSpeciesCount];
        for(int i = 0; i < creatureSpeciesCount; i++)
        {
            buyableLevelsUnlocked[i] = Mathf.Max(i - 2, 0);
        }
    }

    public Sprite GetCreatureSprite(int level)
    {
        return spritesByLevel[level];
    }

    public string GetCreatureName(int level)
    {
        return namesByLevel[level];
    }

    public void MakeSpritesArray(int[] spritesOrder = null)
    {
        if(sprites == null)
            sprites = Resources.LoadAll<Sprite>(textureName);
        spritesByLevel = new Sprite[sprites.Length];

        if (spritesOrder == null || spritesOrder.Length < 1)
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < sprites.Length; i++)
            {
                indexes.Add(i);
            }

            for (int i = 0; i < spritesByLevel.Length; i++)
            {
                int index = Random.Range(0, indexes.Count);
                int sprIndex = indexes[index];
                indexes.RemoveAt(index);
                spritesByLevel[i] = sprites[sprIndex];
            }
        }
        else
        {
            for(int lv = 0; lv < spritesOrder.Length; lv++)
            {
                int spriteRef = spritesOrder[lv];
                spritesByLevel[lv] = sprites[spriteRef];
            }    
        }
    }

    public void MakeNewCreatureNames()
    {
        namesByLevel = new string[spritesByLevel.Length];

        for (int lv = 0; lv < namesByLevel.Length; lv++)
        {
            string name = randomNamesGenerator.GetRandomName();
            namesByLevel[lv] = name[0].ToString().ToUpper() + name.Substring(1);
        }
    }


    public int[] GetSpritesOrder()
    {
        if (sprites == null)
            sprites = Resources.LoadAll<Sprite>(textureName);

        int[] order = new int[spritesByLevel.Length];
        for(int lv = 0; lv < spritesByLevel.Length; lv++)
        {
            for(int i = 0; i < sprites.Length; i++)
            {
                if(spritesByLevel[lv] == sprites[i])
                {
                    order[lv] = i;
                    break;
                }
            }
        }

        return order;
    }
    



#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(CreaturesArea.center, CreaturesArea.size);
    }

#endif

}
