using UnityEngine;
public sealed class DecisionMaker : MonoBehaviour
{
    [SerializeField] int gridHeight = 10;
    [SerializeField] GameObject obstaclePrefab;
    [SerializeField] int obstaclesPerColumn = 1;
    int columnCounter;
    public void EnqueueNext(ColumnQueue queue)
    {
        var data = new ColumnData(columnCounter++, gridHeight);
        for (int i = 0; i < obstaclesPerColumn; i++)
        {
            int row = Random.Range(0, gridHeight);
            data.cells[row] = obstaclePrefab;
        }
        queue.Enqueue(data);
    }
}