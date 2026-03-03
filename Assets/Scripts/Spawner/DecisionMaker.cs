using UnityEngine;
using System.Collections.Generic;

public sealed class DecisionMaker : MonoBehaviour
{
    [SerializeField] DifficultyManager difficultyManager;
    [SerializeField] int gridHeight = 10;
    [SerializeField] GameObject obstaclePrefab;
    [SerializeField] int bufferSize = 50;

    int columnCounter;
    int currentLevel;

    bool[,] mazeGrid;
    int mazeStartColumn;

    void Reset()
    {
        if (difficultyManager == null) difficultyManager = FindFirstObjectByType<DifficultyManager>();
    }

    void Start()
    {
        currentLevel = difficultyManager != null ? difficultyManager.CurrentLevel : 1;
        if (difficultyManager != null)
            difficultyManager.OnLevelChanged += HandleLevelChanged;

        GenerateFullMaze();
    }

    void OnDestroy()
    {
        if (difficultyManager != null)
            difficultyManager.OnLevelChanged -= HandleLevelChanged;
    }

    void HandleLevelChanged(int newLevel)
    {
        currentLevel = newLevel;
    }

    public void EnqueueNext(ColumnQueue queue)
    {
        int localCol = columnCounter - mazeStartColumn;

        if (localCol >= bufferSize - 8)
            ExtendMaze();

        var data = new ColumnData(columnCounter, gridHeight);
        for (int row = 0; row < gridHeight; row++)
        {
            int mc = columnCounter - mazeStartColumn;
            if (mc >= 0 && mc < mazeGrid.GetLength(0) && mazeGrid[mc, row])
                data.cells[row] = obstaclePrefab;
        }

        columnCounter++;
        queue.Enqueue(data);
    }

    void GenerateFullMaze()
    {
        mazeGrid = CarveNewMaze(bufferSize);
        mazeStartColumn = columnCounter;
    }

    void ExtendMaze()
    {
        int existing = mazeGrid.GetLength(0);
        int newSize = existing + bufferSize;
        var extended = new bool[newSize, gridHeight];

        for (int c = 0; c < existing; c++)
            for (int r = 0; r < gridHeight; r++)
                extended[c, r] = mazeGrid[c, r];

        var extension = CarveNewMaze(bufferSize);

        // find open rows at right edge of existing maze to seed the join
        var entryRows = new List<int>();
        for (int r = 0; r < gridHeight; r++)
            if (!mazeGrid[existing - 1, r])
                entryRows.Add(r);

        // stamp extension into extended grid
        for (int c = 0; c < bufferSize; c++)
            for (int r = 0; r < gridHeight; r++)
                extended[existing + c, r] = extension[c, r];

        // carve a connection from right edge of old maze into extension
        foreach (int r in entryRows)
        {
            extended[existing, r] = false;
            extended[existing - 1, r] = false;
        }

        mazeGrid = extended;
    }

    bool[,] CarveNewMaze(int width)
    {
        int roomW = width / 2;
        int roomH = gridHeight / 2;

        var grid = new bool[width, gridHeight];
        for (int c = 0; c < width; c++)
            for (int r = 0; r < gridHeight; r++)
                grid[c, r] = true;

        var visited = new bool[roomW, roomH];
        var stack = new Stack<Vector2Int>();

        int startRY = roomH / 2;
        visited[0, startRY] = true;
        stack.Push(new Vector2Int(0, startRY));

        var dirs = new Vector2Int[] {
            Vector2Int.right, Vector2Int.left,
            Vector2Int.up, Vector2Int.down
        };

        while (stack.Count > 0)
        {
            var cur = stack.Peek();
            grid[cur.x * 2, cur.y * 2] = false;

            ShuffleDirs(dirs);
            bool moved = false;

            foreach (var d in dirs)
            {
                var next = cur + d;
                if (next.x < 0 || next.x >= roomW || next.y < 0 || next.y >= roomH) continue;
                if (visited[next.x, next.y]) continue;

                visited[next.x, next.y] = true;
                grid[cur.x * 2 + d.x, cur.y * 2 + d.y] = false;
                stack.Push(next);
                moved = true;
                break;
            }

            if (!moved) stack.Pop();
        }

        AddExtraPassages(grid, width, roomW, roomH);

        return grid;
    }

    void AddExtraPassages(bool[,] grid, int width, int roomW, int roomH)
    {
        int extra = Mathf.Max(0, 15 - currentLevel);
        for (int i = 0; i < extra; i++)
        {
            int rx = Random.Range(0, roomW - 1);
            int ry = Random.Range(0, roomH);
            grid[rx * 2 + 1, ry * 2] = false;
        }
    }

    void ShuffleDirs(Vector2Int[] arr)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
    }
}