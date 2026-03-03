using UnityEngine;

public sealed class WorldScrollSystem : MonoBehaviour
{
    [SerializeField] WorldContainer world;
    [SerializeField] DifficultyManager difficulty;
    [SerializeField] float baseScrollSpeed = 3f;
    [SerializeField] bool scrolling = true;

    float scrollSpeed;

    public float ScrollSpeed
    {
        get => scrollSpeed;
        set => scrollSpeed = Mathf.Max(0f, value);
    }

    public bool Scrolling
    {
        get => scrolling;
        set => scrolling = value;
    }

    void Reset()
    {
        world = FindFirstObjectByType<WorldContainer>();
        difficulty = FindFirstObjectByType<DifficultyManager>();
    }

    void Start()
    {
        scrollSpeed = baseScrollSpeed;
    }

    void OnEnable()
    {
        if (difficulty != null)
            difficulty.OnLevelChanged += HandleLevelChanged;
    }

    void OnDisable()
    {
        if (difficulty != null)
            difficulty.OnLevelChanged -= HandleLevelChanged;
    }

    void Update()
    {
        if (!scrolling) return;
        if (world == null) return;

        var root = world.Root;
        root.position += Vector3.left * (scrollSpeed * Time.deltaTime);
    }

    public void ResetWorld()
    {
        if (world == null) return;
        world.ResetToStart();
    }

    void HandleLevelChanged(int level)
    {
        scrollSpeed = baseScrollSpeed * (1f + (level / 10f));
    }
}