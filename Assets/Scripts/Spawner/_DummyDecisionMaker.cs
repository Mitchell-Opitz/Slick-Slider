using UnityEngine;

public sealed class _DummyDecisionMaker : MonoBehaviour
{
    [SerializeField] int gridHeight = 10;
    [SerializeField] GameObject obstaclePrefab;
    [SerializeField] int obstaclesPerColumn = 1;

    int columnCounter;

    public ColumnData GenerateNextColumn()
    {
        var data = new ColumnData(columnCounter++, gridHeight);

        for (int i = 0; i < obstaclesPerColumn; i++)
        {
            int row = Random.Range(0, gridHeight);
            data.cells[row] = obstaclePrefab;
        }

        return data;
    }
}
