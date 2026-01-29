using UnityEngine;

public class GridService : MonoBehaviour
{
    public static GridService Instance { get; private set; }

    [SerializeField] GridConfig config;

    public GridSystem Grid { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Grid = new GridSystem(config.cellSize, config.worldOffset);
    }
}
