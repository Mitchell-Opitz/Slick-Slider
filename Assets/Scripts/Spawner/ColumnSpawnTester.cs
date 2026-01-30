using UnityEngine;

public sealed class ColumnSpawnTester : MonoBehaviour
{
    [SerializeField] ColumnSpawner spawner;
    [SerializeField] float spawnInterval = 1f;

    float timer;

    void Reset()
    {
        if (spawner == null) spawner = GetComponent<ColumnSpawner>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            spawner.SpawnNextColumn();
        }
    }
}
