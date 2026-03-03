using UnityEngine;
using System;

public sealed class RunDistanceTracker : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] WorldContainer world;

    GridSystem grid;
    Transform root;

    int lastGridX;
    public int Distance { get; private set; }

    public event Action<int> OnDistanceChanged;

    void Reset()
    {
        if (player == null) player = FindFirstObjectByType<PlayerController>();
        if (world == null) world = FindFirstObjectByType<WorldContainer>();
    }

    void Start()
    {
        grid = GridService.Instance.Grid;
        root = world != null ? world.Root : transform.parent;

        var localPos = player.transform.localPosition;
        var gridPos = grid.WorldToGrid(localPos);

        lastGridX = gridPos.x;
        Distance = 0;
    }

    void Update()
    {
        var localPos = player.transform.localPosition;
        var gridPos = grid.WorldToGrid(localPos);

        if (gridPos.x > lastGridX)
        {
            Distance += (gridPos.x - lastGridX);
            lastGridX = gridPos.x;

            OnDistanceChanged?.Invoke(Distance);
        }
    }
}
