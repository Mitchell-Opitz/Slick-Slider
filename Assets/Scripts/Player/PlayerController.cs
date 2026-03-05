using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float slideSpeed = 12f;
    [SerializeField] PlayerInputReader input;
    [SerializeField] WorldContainer world;
    [SerializeField] PlayerSquashStretch squashStretch;
    [SerializeField] PlayerShake playerShake;
    [SerializeField] ParticleSystem impactParticlesPrefab;

    [Header("Subtle Motion Feel")]
    [SerializeField, Range(0f, 0.35f)] float accelSeconds = 0.08f;
    [SerializeField, Range(0f, 0.35f)] float decelSeconds = 0.08f;
    [SerializeField, Range(0.85f, 1f)] float endSnapDistance = 0.03f;

    GridSystem grid;
    Transform root;

    bool sliding;

    Vector2Int currentGridPos;
    Vector2Int lastSlideDir;
    Vector2 startLocalPos;
    Vector2 targetLocalPos;

    float slideStartTime;
    float slideTotalDist;

    TrailRenderer trail;
    Coroutine disableTrailRoutine;

    void Reset()
    {
        if (world == null) world = FindFirstObjectByType<WorldContainer>();
        if (squashStretch == null) squashStretch = GetComponent<PlayerSquashStretch>();
    }

    void Start()
    {
        grid = GridService.Instance.Grid;
        root = world != null ? world.Root : transform.parent;

        if (root != null && transform.parent != root)
            transform.SetParent(root, true);

        currentGridPos = grid.WorldToGrid(transform.localPosition);
        transform.localPosition = grid.GridToWorld(currentGridPos);

        startLocalPos = transform.localPosition;
        targetLocalPos = transform.localPosition;

        trail = GetComponent<TrailRenderer>();
        if (trail != null) trail.enabled = false;
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
        lastSlideDir = dir;

        startLocalPos = transform.localPosition;
        targetLocalPos = grid.GridToWorld(currentGridPos);

        slideTotalDist = Vector2.Distance(startLocalPos, targetLocalPos);
        slideStartTime = Time.time;

        sliding = true;
        squashStretch?.OnSlideStarted(dir);

        if (trail != null)
        {
            trail.enabled = true;

            if (disableTrailRoutine != null)
                StopCoroutine(disableTrailRoutine);
        }

        return true;
    }

    void SlideUpdate()
    {
        var p = (Vector2)transform.localPosition;
        var toTarget = Vector2.Distance(p, targetLocalPos);

        if (toTarget <= endSnapDistance)
        {
            transform.localPosition = targetLocalPos;
            sliding = false;
            squashStretch?.OnSlideEnded();
            playerShake?.OnSlideEnded(slideTotalDist);
            SpawnImpactParticles();

            if (trail != null)
                disableTrailRoutine = StartCoroutine(DisableTrailAfterTime());

            return;
        }

        var baseStep = slideSpeed * Time.deltaTime;

        var t = Time.time - slideStartTime;
        var accel = accelSeconds <= 0f ? 1f : Mathf.SmoothStep(0.92f, 1f, Mathf.Clamp01(t / accelSeconds));

        var progress = slideTotalDist <= 0.0001f ? 1f : 1f - (toTarget / slideTotalDist);
        var remaining = 1f - progress;

        var decel = decelSeconds <= 0f
            ? 1f
            : Mathf.SmoothStep(0.92f, 1f, Mathf.Clamp01(remaining / (decelSeconds * slideSpeed / Mathf.Max(slideTotalDist, 0.0001f))));

        var step = baseStep * accel * decel;

        transform.localPosition = Vector2.MoveTowards(p, targetLocalPos, step);
    }

    void SpawnImpactParticles()
    {
        if (impactParticlesPrefab == null) return;

        var worldPos = root != null ? (Vector2)root.TransformPoint(targetLocalPos) : targetLocalPos;
        var spawnPos = worldPos + (Vector2)lastSlideDir * 0.5f;
        var rotation = Quaternion.FromToRotation(Vector2.right, (Vector2)lastSlideDir);
        var ps = Instantiate(impactParticlesPrefab, spawnPos, rotation);
        var emission = ps.emission;
        emission.rateOverTime = Mathf.Max(10f, slideTotalDist * 2f);
        ps.Play();
    }

    System.Collections.IEnumerator DisableTrailAfterTime()
    {
        yield return new WaitForSeconds(trail.time);
        trail.enabled = false;
    }

    bool IsBlocked(Vector2Int gridPos)
    {
        var localPos = grid.GridToWorld(gridPos);
        var worldPos = root != null ? (Vector2)root.TransformPoint(localPos) : localPos;

        var hits = Physics2D.OverlapPointAll(worldPos);

        foreach (var h in hits)
        {
            if (h.isTrigger) continue;
            if (h.CompareTag("Obstacle")) return true;
        }

        return false;
    }
}