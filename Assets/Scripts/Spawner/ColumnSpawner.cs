using UnityEngine;
public sealed class ColumnSpawner : MonoBehaviour
{
    [SerializeField] ColumnQueue queue;
    [SerializeField] WorldContainer world;
    [SerializeField] GridConfig config;
    [SerializeField] WorldScrollSystem scrollSystem;
    [SerializeField] float spawnWorldX = 18f;
    GridSystem grid;
    Transform root;
    float distanceTraveled;
    float nextSpawnAt;
    void Start()
    {
        grid = GridService.Instance.Grid;
        root = world != null ? world.Root : transform;
        nextSpawnAt = config.cellSize;
        SpawnNextColumn();
    }
    void Update()
    {
        if (scrollSystem == null || !scrollSystem.Scrolling) return;
        distanceTraveled += scrollSystem.ScrollSpeed * Time.deltaTime;
        if (distanceTraveled >= nextSpawnAt)
        {
            nextSpawnAt += config.cellSize;
            SpawnNextColumn();
        }
    }
    void SpawnNextColumn()
    {
        var data = queue.GetNext();
        int col = Mathf.RoundToInt((spawnWorldX - config.worldOffset.x) / config.cellSize);
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
        int height = 10;
        if (queue != null)
        {
            var peek = queue.PeekNext();
            if (peek.cells != null) height = peek.cells.Length;
        }
        int col = Mathf.RoundToInt((spawnWorldX - config.worldOffset.x) / config.cellSize);
        for (int y = 0; y < height; y++)
        {
            var gridPos = new Vector2Int(col, y);
            var local = new Vector2(
                (gridPos.x * config.cellSize) + config.worldOffset.x,
                (gridPos.y * config.cellSize) + config.worldOffset.y
            );
            Gizmos.DrawWireCube(transform.TransformPoint(local), Vector3.one * 0.9f);
        }
    }
}