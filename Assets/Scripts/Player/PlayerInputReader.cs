using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    [SerializeField] float bufferSeconds = 0.1f;
    [SerializeField] float deadzone = 0.2f;

    Vector2Int heldDir;

    Vector2Int bufferedDir;
    float bufferExpiresAt;

    public bool TryGetMove(out Vector2Int dir)
    {
        // 1) Tap buffer (consumed once, expires quickly)
        if (bufferedDir != Vector2Int.zero && Time.time <= bufferExpiresAt)
        {
            dir = bufferedDir;
            bufferedDir = Vector2Int.zero;
            bufferExpiresAt = 0f;
            return true;
        }

        // 2) Held input (not consumed; stays active while held)
        if (heldDir != Vector2Int.zero)
        {
            dir = heldDir;
            return true;
        }

        dir = Vector2Int.zero;
        bufferedDir = Vector2Int.zero;
        bufferExpiresAt = 0f;
        return false;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        // Read every phase so "holding" counts.
        var v = ctx.ReadValue<Vector2>();
        if (v.sqrMagnitude < deadzone * deadzone) v = Vector2.zero;

        heldDir = ToCardinal(v);

        if (ctx.performed && heldDir != Vector2Int.zero)
        {
            bufferedDir = heldDir;
            bufferExpiresAt = Time.time + bufferSeconds;
        }

        if (ctx.canceled)
        {
            heldDir = Vector2Int.zero;
        }
    }

    static Vector2Int ToCardinal(Vector2 v)
    {
        if (v == Vector2.zero) return Vector2Int.zero;

        var ax = Mathf.Abs(v.x);
        var ay = Mathf.Abs(v.y);

        var x = ax > ay ? (int)Mathf.Sign(v.x) : 0;
        var y = ay > ax ? (int)Mathf.Sign(v.y) : 0;

        return new Vector2Int(x, y);
    }
}
