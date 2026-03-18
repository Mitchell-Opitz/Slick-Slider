using UnityEngine;
using System.Collections;

public sealed class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance { get; private set; }

    [SerializeField] Transform shakeTarget;

    void Awake()
    {
        Instance = this;
        if (shakeTarget == null) shakeTarget = transform;
    }

    public void Shake(float duration, float intensity)
    {
        StopAllCoroutines();
        StartCoroutine(DoShake(duration, intensity));
    }

    IEnumerator DoShake(float duration, float intensity)
    {
        var origin = shakeTarget.localPosition;
        var elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            var strength = intensity * (1f - elapsed / duration);
            shakeTarget.localPosition = origin + (Vector3)Random.insideUnitCircle * strength;
            yield return null;
        }

        shakeTarget.localPosition = origin;
    }
}