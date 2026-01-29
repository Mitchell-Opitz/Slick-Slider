using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(LineRenderer))]
public sealed class Laser : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] LayerMask worldCollisionMask;
    [SerializeField] LayerMask playerMask;
    [SerializeField] Vector3 startOffsetLocal = Vector3.zero;
    [SerializeField] float maxDistance = 1000f;

    [Header("Direction")]
    [SerializeField] bool useUpAxis = true; // true = shoots along transform.up, false = transform.right

    [Header("State")]
    [SerializeField] bool enabledLaser = true;

    LineRenderer lr;
    Vector3 dirWorld;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.enabled = enabledLaser;
        dirWorld = (useUpAxis ? transform.up : transform.right).normalized;
    }

    void Update()
    {
        if (!enabledLaser)
        {
            if (lr.enabled) lr.enabled = false;
            return;
        }

        if (!lr.enabled) lr.enabled = true;

        dirWorld = (useUpAxis ? transform.up : transform.right).normalized;

        var endLocal = FireRaycastEndLocal();
        lr.positionCount = 2;
        lr.SetPosition(0, startOffsetLocal);
        lr.SetPosition(1, endLocal);

        CheckPlayerHit(endLocal);
    }

    public void SetEnabled(bool value)
    {
        enabledLaser = value;
        if (lr != null) lr.enabled = value;
    }

    Vector3 FireRaycastEndLocal()
    {
        var originWorld = transform.position;
        var hit = Physics2D.Raycast(originWorld, dirWorld, maxDistance, worldCollisionMask);

        var endWorld = originWorld + dirWorld * maxDistance;
        if (hit.collider != null) endWorld = hit.point;

        return transform.InverseTransformPoint(endWorld);
    }

    void CheckPlayerHit(Vector3 endLocal)
    {
        var originWorld = transform.position;
        var endWorld = transform.TransformPoint(endLocal);
        var dist = Vector2.Distance(originWorld, endWorld);

        var hit = Physics2D.Raycast(originWorld, dirWorld, dist, playerMask);
        if (hit.collider != null)
        {
            //hook death
        }
    }
}
