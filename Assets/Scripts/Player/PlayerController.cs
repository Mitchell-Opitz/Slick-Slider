using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float slideSpeed = 12f;
    [SerializeField] PlayerInputReader input;

    GridSystem grid;
    bool sliding;
    Vector2Int bufferedDir;
    Vector2Int currentGridPos;
    Vector2 targetWorldPos;

    void Start()
    {
        grid = GridService.Instance.Grid;
        currentGridPos = grid.WorldToGrid(transform.position);
        transform.position = grid.GridToWorld(currentGridPos);
    }

    void Update()
    {
        if (sliding)
        {
            SlideUpdate();
            return;
        }

        if (input.TryConsumeInput(out var dir))
        {
            if (TryStartSlide(dir)) return;
            bufferedDir = dir;
        }
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
        targetWorldPos = grid.GridToWorld(currentGridPos);
        sliding = true;
        return true;
    }

    void SlideUpdate()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetWorldPos,
            slideSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, targetWorldPos) < 0.001f)
        {
            transform.position = targetWorldPos;
            sliding = false;

            if (bufferedDir != Vector2Int.zero)
            {
                var dir = bufferedDir;
                bufferedDir = Vector2Int.zero;
                TryStartSlide(dir);
            }
        }
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
