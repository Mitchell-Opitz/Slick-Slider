using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    Vector2Int moveInput;
    bool hasInput;

    public bool TryConsumeInput(out Vector2Int dir)
    {
        if (!hasInput)
        {
            dir = Vector2Int.zero;
            return false;
        }

        dir = moveInput;
        hasInput = false;
        return true;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        var v = ctx.ReadValue<Vector2>();
        var x = Mathf.Abs(v.x) > Mathf.Abs(v.y) ? Mathf.Sign(v.x) : 0f;
        var y = Mathf.Abs(v.y) > Mathf.Abs(v.x) ? Mathf.Sign(v.y) : 0f;

        moveInput = new Vector2Int((int)x, (int)y);
        hasInput = moveInput != Vector2Int.zero;
    }
}
