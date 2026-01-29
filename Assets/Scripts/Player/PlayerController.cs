using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float slideSpeed = 12f;
    [SerializeField] PlayerInputReader input;

    GridSystem grid;
    bool sliding;

    Vector2Int currentGridPos;
    Vector2 targetWorldPos;

    void Start()
    {
        grid = GridService.Instance.Grid;
        currentGridPos = grid.WorldToGrid(transform.position);
        transform.position = grid.GridToWorld(currentGridPos);
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

        if (Vector2.SqrMagnitude((Vector2)transform.position - targetWorldPos) > 0.000001f) return;

        transform.position = targetWorldPos;
        sliding = false;
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
