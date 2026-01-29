using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float slideSpeed = 12f;
    [SerializeField] PlayerInputReader input;

    [Header("Subtle Motion Feel")]
    [SerializeField, Range(0f, 0.35f)] float accelSeconds = 0.08f;
    [SerializeField, Range(0f, 0.35f)] float decelSeconds = 0.08f;
    [SerializeField, Range(0.85f, 1f)] float endSnapDistance = 0.03f;

    GridSystem grid;
    bool sliding;

    Vector2Int currentGridPos;
    Vector2 startWorldPos;
    Vector2 targetWorldPos;

    float slideStartTime;
    float slideTotalDist;

    void Start()
    {
        grid = GridService.Instance.Grid;
        currentGridPos = grid.WorldToGrid(transform.position);
        transform.position = grid.GridToWorld(currentGridPos);

        startWorldPos = transform.position;
        targetWorldPos = transform.position;
    }

    void Update()
    {
        if (sliding)
        {
            SlideUpdate();
            return;
        }

        if (input.TryGetMove(out var dir))
            TryStartSlide(dir);
    }

    bool TryStartSlide(Vector2Int dir)
    {
        if (dir == Vector2Int.zero) return false;

        var next = currentGridPos + dir;

        while (true)
        {
            if (IsBlocked(next))
            {
                next -= dir;
                break;
            }

            next += dir;
        }

        if (next == currentGridPos) return false;

        currentGridPos = next;

        startWorldPos = transform.position;
        targetWorldPos = grid.GridToWorld(currentGridPos);

        slideTotalDist = Vector2.Distance(startWorldPos, targetWorldPos);
        slideStartTime = Time.time;

        sliding = true;
        return true;
    }

    void SlideUpdate()
    {
        var p = (Vector2)transform.position;
        var toTarget = Vector2.Distance(p, targetWorldPos);

        if (toTarget <= endSnapDistance)
        {
            transform.position = targetWorldPos;
            sliding = false;
            return;
        }

        var baseStep = slideSpeed * Time.deltaTime;

        // Very subtle accel at start, subtle decel near end.
        var t = Time.time - slideStartTime;

        var accel = accelSeconds <= 0f ? 1f : Mathf.SmoothStep(0.92f, 1f, Mathf.Clamp01(t / accelSeconds));

        var progress = slideTotalDist <= 0.0001f ? 1f : 1f - (toTarget / slideTotalDist);
        var remaining = 1f - progress;

        var decel = decelSeconds <= 0f
            ? 1f
            : Mathf.SmoothStep(0.92f, 1f, Mathf.Clamp01(remaining / (decelSeconds * slideSpeed / Mathf.Max(slideTotalDist, 0.0001f))));

        var step = baseStep * accel * decel;

        transform.position = Vector2.MoveTowards(p, targetWorldPos, step);
    }

    bool IsBlocked(Vector2Int gridPos)
    {
        var worldPos = grid.GridToWorld(gridPos);
        var hits = Physics2D.OverlapPointAll(worldPos);

        foreach (var h in hits)
        {
            if (h.isTrigger) continue;
            if (h.CompareTag("Obstacle")) return true;
        }

        return false;
    }
}
