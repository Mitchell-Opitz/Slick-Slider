using UnityEngine;
using TMPro;

public sealed class HUDLevelDisplay : MonoBehaviour
{
    [SerializeField] DifficultyManager difficulty;
    [SerializeField] TMP_Text text;

    void Reset()
    {
        if (difficulty == null) difficulty = FindFirstObjectByType<DifficultyManager>();
        if (text == null) text = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        if (difficulty != null)
            difficulty.OnLevelChanged += HandleLevelChanged;
    }

    void OnDisable()
    {
        if (difficulty != null)
            difficulty.OnLevelChanged -= HandleLevelChanged;
    }

    void Start()
    {
        if (difficulty != null)
            text.text = "Level: " + difficulty.CurrentLevel.ToString();
    }

    void HandleLevelChanged(int newLevel)
    {
        text.text = "Level: " + newLevel.ToString();
    }
}
