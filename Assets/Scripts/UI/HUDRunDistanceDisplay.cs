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

    void Update()
    {
        text.text = "Distance: " + tracker.Distance.ToString();
    }
}
