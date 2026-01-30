using UnityEngine;
using TMPro;

public sealed class HUDRunDistanceDisplay : MonoBehaviour
{
    [SerializeField] RunDistanceTracker tracker;
    [SerializeField] TMP_Text text;

    void Reset()
    {
        if (tracker == null) tracker = FindFirstObjectByType<RunDistanceTracker>();
        if (text == null) text = GetComponent<TMP_Text>();
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

    void HandleDistanceChanged(int newDistance)
    {
        text.text = "Distance: " + newDistance.ToString();
    }
}
