using UnityEngine;

public sealed class WorldScrollSystem : MonoBehaviour
{
    [SerializeField] WorldContainer world;
    [SerializeField] float scrollSpeed = 3f;
    [SerializeField] bool scrolling = true;

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
}
