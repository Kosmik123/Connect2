using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("To Link")]
    public GameObject creaturePrefab;
    public Transform creaturesContainer;

    private Settings settings;
    // Start is called before the first frame update
    void Start()
    {
        settings = Settings.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateCreature()
    {
        GameObject creatureObj = Instantiate(creaturePrefab, creaturesContainer);
        creatureObj.transform.position = new Vector3(
            Random.Range(settings.creaturesArea.xMin, settings.creaturesArea.xMax),
            Random.Range(settings.creaturesArea.yMin, settings.creaturesArea.yMax));
        creatureObj.name = "Creature 0";
    }
}
