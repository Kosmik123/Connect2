using UnityEngine;

public class CreatureData : MonoBehaviour
{
    public static AnimationCurve scaleCurve;
    public static int maxLevel;

    private Settings settings;

    [Header("To Link")]
    [SerializeField]
    private new SpriteRenderer renderer;
    [SerializeField]
    private new Collider2D collider;
    public Collider2D Collider => collider;

    [Header("States")]
    [SerializeField]
    private int level; // from 0 to maxLevel
    public int Level => level;

    [SerializeField]
    private bool isMoving;
    public bool IsMoving
    {
        get => isMoving;
        set 
        { 
            isMoving = value; 
        }
    }

    private void Awake()
    {
        collider = GetComponent<Collider2D>();
    }

    public void Init(int level)
    {
        this.level = level;
    }

    void Start()
    {
        settings = GameController.main.Settings;
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
        isMoving = false;
        level += 1;
        Refresh();
    }
}
