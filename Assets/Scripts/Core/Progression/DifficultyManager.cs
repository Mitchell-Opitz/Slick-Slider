using UnityEngine;
using System;

public sealed class DifficultyManager : MonoBehaviour
{
    [SerializeField] RunDistanceTracker tracker;
    [SerializeField, Min(1)] int unitsPerLevel = 100;

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
        return Mathf.FloorToInt((float)distance / unitsPerLevel) + 1;
    }
}