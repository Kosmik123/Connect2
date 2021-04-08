using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CreatureData))]
public class CreatureController : MonoBehaviour
{
    public static Rect movementRange;



    private CreatureData creature;
    
    [Header("Properties")]
    [Range(0,1)]
    public float moveSpeed;
    
    [Header("Movement States")]
    public bool isMoving;
    public Vector3 targetPosition;

    [Header("States")]
    public bool isDragged;
    public Vector3 relativePos;
    public Collider2D[] touchedColliders = new Collider2D[5];

    private void Awake()
    {
        creature = GetComponent<CreatureData>();
    }

    void Start()
    {
        StartCoroutine("MoveRandomlyCo");
    }

    void Update()
    {
        if (isMoving)
            DoMove();

        if (isDragged)
            DoDrag();
    }

    IEnumerator MoveRandomlyCo()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 8f));
            if(!isMoving && !isDragged)
                StartRandomMove();
        }
    }
    
    void StartRandomMove()
    {
        targetPosition = new Vector3(
            Random.Range(movementRange.xMin, movementRange.xMax),
            Random.Range(movementRange.yMin, movementRange.yMax));
        isMoving = true;
    }

    void DoMove()
    {
        if (isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed);
            if ((transform.position - targetPosition).sqrMagnitude < 0.01f)
                isMoving = false;
        }
    }

    void DoDrag()
    {
        transform.position = relativePos + Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }


    private void OnMouseDown()
    {
        relativePos = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isMoving = false;
        isDragged = true;
    }

    private void OnMouseUp()
    {
        isDragged = false;

        touchedColliders = new Collider2D[1];
        creature.collider.OverlapCollider(new ContactFilter2D(), touchedColliders);
        if(touchedColliders[0] != null)
        {
            var otherCreature = touchedColliders[0].GetComponent<CreatureData>();
            if(otherCreature != null && otherCreature.level == creature.level) 
            {
                Destroy(gameObject);
                otherCreature.LevelUp();
                otherCreature.gameObject.name = "Creature " + otherCreature.level;
            }
        }            
    }

}

