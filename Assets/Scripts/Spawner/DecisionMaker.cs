using UnityEngine;

public sealed class DecisionMaker : MonoBehaviour
{
    [SerializeField] DifficultyManager difficultyManager;
    [SerializeField] int gridHeight = 18;
    [SerializeField] GameObject obstaclePrefab;
    [SerializeField] int bufferSize = 50;
    [SerializeField, Range(0f, 1f)] float bumperDensity = 0.25f;

    int columnCounter;
    int currentLevel;

    bool[,] bumpers;
    int bufferStartColumn;

    void Reset()
    {
        if (difficultyManager == null) difficultyManager = FindFirstObjectByType<DifficultyManager>();
    }

    void Start()
    {
        currentLevel = difficultyManager != null ? difficultyManager.CurrentLevel : 1;
        if (difficultyManager != null)
            difficultyManager.OnLevelChanged += HandleLevelChanged;

        bumpers = GenerateBumpers(bufferSize);
        bufferStartColumn = columnCounter;
    }

    void OnDestroy()
    {
        if (difficultyManager != null)
            difficultyManager.OnLevelChanged -= HandleLevelChanged;
    }

    void HandleLevelChanged(int newLevel) => currentLevel = newLevel;

    public void EnqueueNext(ColumnQueue queue)
    {
        int local = columnCounter - bufferStartColumn;

        if (local >= bumpers.GetLength(0) - 8)
        {
            var extended = new bool[bumpers.GetLength(0) + bufferSize, gridHeight];
            for (int c = 0; c < bumpers.GetLength(0); c++)
                for (int r = 0; r < gridHeight; r++)
                    extended[c, r] = bumpers[c, r];

            var next = GenerateBumpers(bufferSize);
            for (int c = 0; c < bufferSize; c++)
                for (int r = 0; r < gridHeight; r++)
                    extended[bumpers.GetLength(0) + c, r] = next[c, r];

            bumpers = extended;
        }

        local = columnCounter - bufferStartColumn;
        var data = new ColumnData(columnCounter, gridHeight);

        if (local >= 0 && local < bumpers.GetLength(0))
            for (int row = 0; row < gridHeight; row++)
                if (bumpers[local, row])
                    data.cells[row] = obstaclePrefab;

        columnCounter++;
        queue.Enqueue(data);
    }

    // Place bumpers only on even-indexed cells so there's always
    // at least one empty cell between any two bumpers in the same row/column.
    // This guarantees the player can always slide through gaps.
    bool[,] GenerateBumpers(int width)
    {
        var result = new bool[width, gridHeight];
        float density = 0.08f + currentLevel * 0.005f;

        for (int c = 0; c < width; c++)
            for (int r = 0; r < gridHeight; r++)
                if (Random.value < density)
                    result[c, r] = true;

        return result;
    }
}