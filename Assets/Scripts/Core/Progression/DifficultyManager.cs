using UnityEngine;
using System;

public sealed class DifficultyManager : MonoBehaviour
{
    [SerializeField] RunDistanceTracker tracker;

    public int CurrentLevel { get; private set; } = 1;

    public event Action<int> OnLevelChanged;

    void Reset()
    {
        if (tracker == null) tracker = FindFirstObjectByType<RunDistanceTracker>();
    }

    void OnEnable()
    {
        if (tracker != null)
            tracker.OnDistanceChanged += HandleDistanceChanged;
    }

    void OnDisable()
    {
        if (tracker != null)
            tracker.OnDistanceChanged -= HandleDistanceChanged;
    }

    void HandleDistanceChanged(int distance)
    {
        int newLevel = CalculateLevel(distance);

        if (newLevel > CurrentLevel)
        {
            CurrentLevel = newLevel;
            OnLevelChanged?.Invoke(CurrentLevel);
        }
    }

    int CalculateLevel(int distance)
    {
        // Level 1 = 0–99
        // Level 2 = 100–199
        // Level 3 = 200–299
        // etc.
        return Mathf.FloorToInt(distance / 100f) + 1;
    }
}
