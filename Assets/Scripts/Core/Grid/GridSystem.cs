using UnityEngine;

public class GridSystem
{
    readonly float cellSize;
    readonly Vector2 offset;

    public GridSystem(float cellSize, Vector2 offset)
    {
        this.cellSize = cellSize;
        this.offset = offset;
    }

    public Vector2Int WorldToGrid(Vector2 worldPos)
    {
        var p = worldPos - offset;
        return new Vector2Int(
            Mathf.RoundToInt(p.x / cellSize),
            Mathf.RoundToInt(p.y / cellSize)
        );
    }

    public Vector2 GridToWorld(Vector2Int gridPos)
    {
        return new Vector2(
            (gridPos.x * cellSize) + offset.x,
            (gridPos.y * cellSize) + offset.y
        );
    }

    public Vector2 Snap(Vector2 worldPos)
    {
        return GridToWorld(WorldToGrid(worldPos));
    }

    public bool IsAligned(Vector2 worldPos)
    {
        return Vector2.Distance(worldPos, Snap(worldPos)) < 0.001f;
    }

    public Vector2Int Neighbor(Vector2Int pos, Vector2Int dir)
    {
        return pos + dir;
    }
}
