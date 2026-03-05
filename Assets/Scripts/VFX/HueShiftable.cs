using UnityEngine;

public sealed class HueShiftable : MonoBehaviour
{
    [SerializeField] Color baseColor = Color.white;
    [SerializeField] bool captureOnAwake = true;

    SpriteRenderer spriteRenderer;
    LineRenderer lineRenderer;
    LaserVisuals laserVisuals;
    Camera cam;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lineRenderer = GetComponent<LineRenderer>();
        laserVisuals = GetComponent<LaserVisuals>();
        cam = GetComponent<Camera>();

        if (captureOnAwake)
            baseColor = ReadCurrentColor();
    }

    void OnEnable() => HueShiftManager.Instance?.Register(this);
    void OnDisable() => HueShiftManager.Instance?.Unregister(this);

    public void ApplyShift(float degrees)
    {
        WriteColor(HueShift.Shift(baseColor, degrees));
    }

    Color ReadCurrentColor()
    {
        if (laserVisuals != null) return laserVisuals.LaserColor;
        if (spriteRenderer != null) return spriteRenderer.color;
        if (lineRenderer != null) return lineRenderer.startColor;
        if (cam != null) return cam.backgroundColor;
        return baseColor;
    }

    void WriteColor(Color c)
    {
        if (laserVisuals != null) { laserVisuals.SetColor(c); return; }
        if (spriteRenderer != null) { spriteRenderer.color = c; return; }
        if (lineRenderer != null) { lineRenderer.startColor = c; lineRenderer.endColor = c; return; }
        if (cam != null) { cam.backgroundColor = c; }
    }
}