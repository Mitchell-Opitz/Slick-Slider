using UnityEngine;

public sealed class PlayerShake : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] AnimationCurve intensityCurve = AnimationCurve.Linear(0, 0, 20, 2f);
    [SerializeField] float shakeDuration = 0.2f;
    [SerializeField] float minimumDistance = 3f;

    void Reset()
    {
        player = FindFirstObjectByType<PlayerController>();
    }

    public void OnSlideEnded(float distanceTraveled)
    {
        if (distanceTraveled < minimumDistance) return;

        var intensity = intensityCurve.Evaluate(distanceTraveled);
        ScreenShake.Instance?.Shake(shakeDuration, intensity);
    }
}