using System.Collections;
using UnityEngine;

public sealed class PlayerShake : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] AnimationCurve intensityCurve = AnimationCurve.Linear(0, 0, 20, 2f);
    [SerializeField] float shakeDuration = 0.2f;
    [SerializeField] float hapticDuration = 0.15f;
    [SerializeField] float minimumDistance = 3f;
    [SerializeField] float hapticMinDistance = 4f;
    [SerializeField] float hapticMaxDistance = 15f;
    [SerializeField, Range(0f, 1f)] float hapticMinStrength = 0.01f;
    [SerializeField, Range(0f, 1f)] float hapticMaxStrength = 0.15f;

    Coroutine hapticRoutine;

    void Reset()
    {
        player = FindFirstObjectByType<PlayerController>();
    }

    public void OnSlideEnded(float distanceTraveled)
    {
        if (distanceTraveled < minimumDistance) return;

        ScreenShake.Instance?.Shake(shakeDuration, intensityCurve.Evaluate(distanceTraveled));

        if (hapticRoutine != null) StopCoroutine(hapticRoutine);
        hapticRoutine = StartCoroutine(HapticPulse(distanceTraveled));
    }

    public void OnSlideCleared()
    {
        HapticManager.Instance?.ClearSlide();
    }

    IEnumerator HapticPulse(float distanceTraveled)
    {
        var t = Mathf.InverseLerp(hapticMinDistance, hapticMaxDistance, distanceTraveled);
        var intensity = Mathf.Lerp(hapticMinStrength, hapticMaxStrength, t);
        HapticManager.Instance?.SetSlide(intensity);
        yield return new WaitForSeconds(hapticDuration);
        HapticManager.Instance?.ClearSlide();
        hapticRoutine = null;
    }
}