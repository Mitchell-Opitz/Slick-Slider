using UnityEngine;
using System.Collections.Generic;

public sealed class HueShiftManager : MonoBehaviour
{
    public static HueShiftManager Instance { get; private set; }

    [SerializeField] DifficultyManager difficultyManager;
    [SerializeField] float degreesPerLevel = 15f;

    readonly List<HueShiftable> shiftables = new();

    void Reset()
    {
        if (difficultyManager == null) difficultyManager = FindFirstObjectByType<DifficultyManager>();
    }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnEnable()
    {
        if (difficultyManager != null)
            difficultyManager.OnLevelChanged += HandleLevelChanged;
    }

    void OnDisable()
    {
        if (difficultyManager != null)
            difficultyManager.OnLevelChanged -= HandleLevelChanged;
    }

    public void Register(HueShiftable s) => shiftables.Add(s);
    public void Unregister(HueShiftable s) => shiftables.Remove(s);

    void HandleLevelChanged(int newLevel)
    {
        float totalDegrees = (newLevel - 1) * degreesPerLevel;
        foreach (var s in shiftables)
            s.ApplyShift(totalDegrees);
    }
}