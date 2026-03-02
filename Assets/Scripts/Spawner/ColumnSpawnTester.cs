using UnityEngine;
public sealed class ColumnSpawnTester : MonoBehaviour
{
    [SerializeField] ColumnSpawner spawner;
    [SerializeField] WorldScrollSystem scrollSystem;
    [SerializeField] GridConfig config;
    float distanceTraveled;
    float nextSpawnAt;

    void Start()
    {
        nextSpawnAt = config.cellSize;
        spawner.SpawnNextColumn();
    }

    void Update()
    {
        if (scrollSystem == null || !scrollSystem.Scrolling) return;
        distanceTraveled += scrollSystem.ScrollSpeed * Time.deltaTime;
        if (distanceTraveled >= nextSpawnAt)
        {
            nextSpawnAt += config.cellSize;
            spawner.SpawnNextColumn();
        }
    }
}