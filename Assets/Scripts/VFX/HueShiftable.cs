using UnityEngine;

public sealed class HueShiftable : MonoBehaviour
{
    [SerializeField] Color baseColor = Color.white;
    [SerializeField] Color baseColorSecondary = Color.white;
    [SerializeField] bool captureOnAwake = true;

    SpriteRenderer spriteRenderer;
    LineRenderer lineRenderer;
    LaserVisuals laserVisuals;
    ParticleSystem particles;
    Camera cam;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lineRenderer = GetComponent<LineRenderer>();
        laserVisuals = GetComponent<LaserVisuals>();
        particles = GetComponent<ParticleSystem>();
        cam = GetComponent<Camera>();

        if (captureOnAwake)
            CaptureBaseColors();
    }

    void Start()
    {
        HueShiftManager.Instance?.Register(this);
    }

    void OnDisable() => HueShiftManager.Instance?.Unregister(this);

    public void ApplyShift(float degrees)
    {
        Debug.Log($"ApplyShift {gameObject.name} degrees={degrees} particles={particles != null}");

        var primary = HueShift.Shift(baseColor, degrees);

        if (laserVisuals != null) { laserVisuals.SetColor(primary); return; }
        if (spriteRenderer != null) { spriteRenderer.color = primary; return; }
        if (lineRenderer != null) { lineRenderer.startColor = primary; lineRenderer.endColor = primary; return; }
        if (particles != null)
        {
            var main = particles.main;
            var secondary = HueShift.Shift(baseColorSecondary, degrees);
            main.startColor = new ParticleSystem.MinMaxGradient(primary, secondary);
            return;
        }
        if (cam != null) { cam.backgroundColor = primary; }
    }

    void CaptureBaseColors()
    {
        if (laserVisuals != null) { baseColor = laserVisuals.LaserColor; return; }
        if (spriteRenderer != null) { baseColor = spriteRenderer.color; return; }
        if (lineRenderer != null) { baseColor = lineRenderer.startColor; return; }
        if (particles != null)
        {
            var main = particles.main;
            baseColor = main.startColor.colorMin;
            baseColorSecondary = main.startColor.colorMax;
            return;
        }
        if (cam != null) { baseColor = cam.backgroundColor; }
    }
}