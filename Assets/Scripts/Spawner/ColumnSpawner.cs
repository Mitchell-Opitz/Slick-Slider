using UnityEngine;

public sealed class ColumnSpawner : MonoBehaviour
{
    [SerializeField] ColumnQueue queue;
    [SerializeField] WorldContainer world;
    [SerializeField] GridConfig config;

    [SerializeField] float spawnWorldX = 18f;

    GridSystem grid;
    Transform root;

    void Start()
    {
        grid = GridService.Instance.Grid;
        root = world != null ? world.Root : transform;
    }

    public void SpawnNextColumn()
    {
        var data = queue.GetNext();

        int col = Mathf.RoundToInt((spawnWorldX - config.worldOffset.x) / config.cellSize);
        float yOffset = transform.position.y;

        for (int y = 0; y < data.cells.Length; y++)
        {
            var prefab = data.cells[y];
            if (prefab == null) continue;

            var gridPos = new Vector2Int(col, y);
            var worldPos = grid.GridToWorld(gridPos);

            Instantiate(prefab, worldPos, Quaternion.identity, root);
        }
    }

    void OnDrawGizmos()
    {
        if (config == null) return;

        Gizmos.color = Color.yellow;

        int height = queue != null ? queue.PeekNext().cells.Length : 10;

        int col = Mathf.RoundToInt((spawnWorldX - config.worldOffset.x) / config.cellSize);

        for (int y = 0; y < height; y++)
        {
            var gridPos = new Vector2Int(col, y);
            var local = new Vector2(
                (gridPos.x * config.cellSize) + config.worldOffset.x,
                (gridPos.y * config.cellSize) + config.worldOffset.y
            );

            var worldPos = transform.TransformPoint(local);
            Gizmos.DrawWireCube(worldPos, Vector3.one * 0.9f);
        }
    }
}
