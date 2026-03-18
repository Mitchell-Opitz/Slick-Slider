using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class ScreenFlash : MonoBehaviour
{
    public static ScreenFlash Instance { get; private set; }

    [SerializeField] Image overlay;
    [SerializeField] float flashDuration = 0.3f;
    [SerializeField] AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    Coroutine flashRoutine;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Flash(Color color)
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(DoFlash(color));
    }

    IEnumerator DoFlash(Color color)
    {
        var elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            var a = fadeCurve.Evaluate(elapsed / flashDuration);
            overlay.color = new Color(color.r, color.g, color.b, a);
            yield return null;
        }
        overlay.color = Color.clear;
        flashRoutine = null;
    }
}