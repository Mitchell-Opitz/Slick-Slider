using UnityEngine;

public sealed class WorldContainer : MonoBehaviour
{
    [SerializeField] Transform root;

    Vector3 startPos;

    public Transform Root => root != null ? root : transform;

    void Awake()
    {
        startPos = Root.position;
    }

    public void ResetToStart()
    {
        Root.position = startPos;
    }
}
