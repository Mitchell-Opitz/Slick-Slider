using UnityEngine;
using System.Collections.Generic;

public sealed class HueShiftManager : MonoBehaviour
{
    public static HueShiftManager Instance { get; private set; }
    [SerializeField] DifficultyManager difficultyManager;
    [SerializeField] float degreesPerLevel = 15f;

    static readonly Color obstacleBaseColor = new Color(0x37 / 255f, 0x7F / 255f, 0xE2 / 255f);

    readonly List<HueShiftable> shiftables = new();
    float currentDegrees;

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

    public void Register(HueShiftable s)
    {
        shiftables.Add(s);
        s.ApplyShift(currentDegrees);
    }

    public void Unregister(HueShiftable s) => shiftables.Remove(s);

    void HandleLevelChanged(int newLevel)
    {
        currentDegrees = (newLevel - 1) * degreesPerLevel;
        foreach (var s in shiftables)
            s.ApplyShift(currentDegrees);

        ScreenFlash.Instance?.Flash(HueShift.Shift(obstacleBaseColor, currentDegrees));
    }
}