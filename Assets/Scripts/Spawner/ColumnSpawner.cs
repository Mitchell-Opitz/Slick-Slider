using UnityEngine;

public sealed class ColumnSpawner : MonoBehaviour
{
    [SerializeField] ColumnQueue queue;
    [SerializeField] WorldContainer world;

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

        for (int y = 0; y < data.cells.Length; y++)
        {
            var prefab = data.cells[y];
            if (prefab == null) continue;

            var gridPos = new Vector2Int(data.columnIndex, y);
            var localPos = grid.GridToWorld(gridPos);

            Instantiate(prefab, root.TransformPoint(localPos), Quaternion.identity, root);
        }
    }
}
