using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(LineRenderer))]
public sealed class LaserVisuals : MonoBehaviour
{
    [Header("Color")]
    [SerializeField] Color laserColor = Color.red;

    [Header("Jagged")]
    [SerializeField] float segmentLength = 0.125f;
    [SerializeField] float jaggedness = 0.2f;

    LineRenderer lr;

    public Color LaserColor => laserColor;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        ApplyColor();
    }

    void LateUpdate()
    {
        if (!lr.enabled || lr.positionCount < 2) return;

        var start = lr.GetPosition(0);
        var end = lr.GetPosition(1);

        ApplyJagged(start, end);
    }

    void ApplyColor()
    {
        lr.startColor = laserColor;
        lr.endColor = laserColor;

        if (lr.material != null && lr.material.HasProperty("_Color"))
            lr.material.SetColor("_Color", laserColor);
    }

    void ApplyJagged(Vector3 start, Vector3 end)
    {
        var dist = Vector3.Distance(start, end);

        // Force >= 2 segments so we actually get interior points (jaggedness visible).
        var segLen = Mathf.Max(0.0001f, segmentLength);
        var segments = Mathf.Max(2, Mathf.RoundToInt(dist / segLen));

        var points = new Vector3[segments + 1];
        points[0] = start;
        points[segments] = end;

        var dir = (end - start).normalized;
        var perp = Vector3.Cross(dir, Vector3.forward).normalized;

        for (int i = 1; i < segments; i++)
        {
            var t = (float)i / segments;
            var p = Vector3.Lerp(start, end, t);
            p += perp * Random.Range(-jaggedness, jaggedness);
            points[i] = p;
        }

        lr.positionCount = points.Length;
        lr.SetPositions(points);
    }

    public void SetColor(Color c)
    {
        laserColor = c;
        ApplyColor();
    }
}
