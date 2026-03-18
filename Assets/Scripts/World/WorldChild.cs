using UnityEngine;

public sealed class WorldChild : MonoBehaviour
{
    [SerializeField] WorldContainer world;

    void Reset()
    {
        world = FindFirstObjectByType<WorldContainer>();
    }

    void OnEnable()
    {
        if (world == null) return;
        transform.SetParent(world.Root, true);
    }
}
