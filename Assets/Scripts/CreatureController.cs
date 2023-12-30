using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CreatureData))]
public class CreatureController : MonoBehaviour
{
    public static Rect movementRange;
    public static bool canCreturesBeMoved = true;

    private CreatureData _creature;
    public CreatureData Creature
    {
        get
        {
            if (_creature == null)
                _creature = GetComponent<CreatureData>();
            return _creature;
        }
    }

    private Animator animator;
    private GameController gameController;

    [Header("Properties")]
    [SerializeField, Range(0,1)]
    private float moveSpeed;
    [SerializeField]
    private float fusionRange;
    
    [Header("Movement States")]
    [SerializeField]
    private Vector3 targetPosition;

    [Header("States")]
    [SerializeField]
    private bool isDragged;
    [SerializeField]
    private Vector3 relativePos;
    private Collider2D[] touchedColliders = new Collider2D[1];

    [SerializeField]
    private bool isConnecting;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        gameController = GameController.main;
        StartCoroutine(nameof(MoveRandomlyCo));
    }

    void Update()
    {
        if (Creature.IsMoving)
            DoMove();

        if (isDragged)
            DoDrag();

        animator.SetBool("Moving", Creature.IsMoving);
    }

    IEnumerator MoveRandomlyCo()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 8f));
            if(!Creature.IsMoving && !isDragged && Creature.Level > 0)
                StartRandomMove();
        }
    }
    
    void StartRandomMove()
    {
        targetPosition = new Vector3(
            Random.Range(movementRange.xMin, movementRange.xMax),
            Random.Range(movementRange.yMin, movementRange.yMax));
        Creature.IsMoving = true;
    }

    public void StartMoveToTarget(Vector3 targetPos)
    {
        targetPosition = targetPos;
        Creature.IsMoving = true;
    }


    void DoMove()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed);
        if ((transform.position - targetPosition).sqrMagnitude < 0.1f)
            Creature.IsMoving = false;
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
            Creature.IsMoving = false;
            isDragged = true;
        }
    }

    private void OnMouseUpAsButton()
    {
        if (canCreturesBeMoved)
        {
            Creature.IsMoving = false;
            gameController.AddMoneyFromCreature(Creature);
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
                    var collider = touchedColliders[i];
                    if (collider && collider != Creature.Collider)
                    {
                        var otherCreature = collider.GetComponent<CreatureData>();
                        if (otherCreature != null && otherCreature.Level == Creature.Level)
                        {
                            otherCreature.LevelUp();
                            otherCreature.gameObject.name = "Creature " + otherCreature.Level;
                            gameController.UnlockCreature(otherCreature.Level);

                            gameController.RemoveCreature(Creature);
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

