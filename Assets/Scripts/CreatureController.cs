using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CreatureData))]
public class CreatureController : MonoBehaviour
{
    public static Rect movementRange;
    public static bool canCreturesBeMoved = true;

    //TO LINK
    [HideInInspector]
    public CreatureData creature;
    private Animator animator;
    private GameController gameController;

    [Header("Properties")]
    [Range(0,1)]
    public float moveSpeed;
    public float fusionRange;
    
    [Header("Movement States")]
    public Vector3 targetPosition;

    [Header("States")]
    public bool isDragged;
    private Vector3 relativePos;
    public Collider2D[] touchedColliders = new Collider2D[1];

    public bool isConnecting;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        creature = GetComponent<CreatureData>();
    }

    void Start()
    {
        gameController = GameController.main;
        StartCoroutine(nameof(MoveRandomlyCo));
    }

    void Update()
    {
        if (creature.isMoving)
            DoMove();

        if (isDragged)
            DoDrag();

        animator.SetBool("Moving", creature.isMoving);
    }

    IEnumerator MoveRandomlyCo()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 8f));
            if(!creature.isMoving && !isDragged && creature.level > 0)
                StartRandomMove();
        }
    }
    
    void StartRandomMove()
    {
        targetPosition = new Vector3(
            Random.Range(movementRange.xMin, movementRange.xMax),
            Random.Range(movementRange.yMin, movementRange.yMax));
        creature.isMoving = true;
    }

    public void StartMoveToTarget(Vector3 targetPos)
    {
        targetPosition = targetPos;
        creature.isMoving = true;
    }


    void DoMove()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed);
        if ((transform.position - targetPosition).sqrMagnitude < 0.1f)
            creature.isMoving = false;
    }

    void DoDrag()
    { 
        relativePos = Vector3.Lerp(relativePos, relativePos.z * Vector3.forward, 0.05f);
        transform.position = relativePos + 
            Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }


    private void OnMouseDown()
    {
        if (canCreturesBeMoved)
        {
            relativePos = transform.position -
            Camera.main.ScreenToWorldPoint(Input.mousePosition);
            creature.isMoving = false;
            isDragged = true;
        }
    }

    private void OnMouseUpAsButton()
    {
        if (canCreturesBeMoved)
        {
            creature.isMoving = false;
            gameController.AddMoneyFromCreature(creature);
            animator.SetTrigger("Shake");
        }
    }

    private void OnMouseUp()
    {
        isDragged = false;
        if (canCreturesBeMoved)
        {
            touchedColliders = Physics2D.OverlapCircleAll(transform.position, fusionRange * transform.localScale.x);
            if (touchedColliders.Length > 0)
            {
                for (int i = 0; i < touchedColliders.Length; i++)
                {
                    if (touchedColliders[i] != null && touchedColliders[i] != creature.collider)
                    {
                        var otherCreature = touchedColliders[i].GetComponent<CreatureData>();
                        if (otherCreature != null && otherCreature.level == creature.level)
                        {
                            otherCreature.LevelUp();
                            otherCreature.gameObject.name = "Creature " + otherCreature.level;
                            gameController.CheckCongratulations(otherCreature.level);

                            gameController.creatures.Remove(creature);
                            Destroy(gameObject);
                            break;
                        }
                    }
                }
            }
            gameController.RecalculateIncomeSpeedAndMaxLevels();
        }
    }
}

