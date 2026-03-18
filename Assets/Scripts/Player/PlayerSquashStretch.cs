using UnityEngine;

public sealed class PlayerSquashStretch : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField, Range(0f, 1f)] float squashStretchSeconds = 0.12f;
    [SerializeField] float stretchAmount = 1.35f;
    [SerializeField] float squashAmount = 0.7f;

    Vector3 targetScale;
    Vector2Int lastDir;

    void Reset()
    {
        player = GetComponent<PlayerController>();
        if (player == null) player = FindFirstObjectByType<PlayerController>();
    }

    void Start()
    {
        targetScale = Vector3.one;
    }

    public void OnSlideStarted(Vector2Int dir)
    {
        lastDir = dir;
        bool horizontal = dir.x != 0;

        targetScale = horizontal
            ? new Vector3(stretchAmount, squashAmount, 1f)
            : new Vector3(squashAmount, stretchAmount, 1f);
    }

    public void OnSlideEnded()
    {
        targetScale = Vector3.one;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            1f - Mathf.Pow(0.0001f, Time.deltaTime / Mathf.Max(squashStretchSeconds, 0.0001f))
        );
    }
}