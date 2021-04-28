﻿using System.Collections;
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

    private Sprite[] sprites;

    private void Awake()
    {
        main = this;
        sprites = Resources.LoadAll<Sprite>(textureName);
    }

    private void Start()
    {
        CreatureData.scaleCurve = scaleByLevel;
        CreatureData.maxLevel = maxLevel;        
        
        CreatureController.movementRange = creaturesArea;
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
        Gizmos.DrawWireCube(creaturesArea.center, creaturesArea.size);
    }

#endif

}
