using UnityEngine;

public sealed class LaserProximity : MonoBehaviour
{
    [SerializeField] Transform laser;
    [SerializeField] float maxDistance = 20f;
    [SerializeField] float minDistance = 1f;
    [SerializeField, Range(0f, 1f)] float hapticAtMaxDistance = 0.10f;
    [SerializeField, Range(0f, 1f)] float hapticAtMinDistance = 0.75f;
    void Start()
    {
        if (laser == null)
            laser = FindFirstObjectByType<Laser>()?.transform;
    }

    void Update()
    {
        if (laser == null) return;

        var dist = Mathf.Abs(transform.position.x - laser.transform.position.x);

        if (dist > maxDistance)
        {
            HapticManager.Instance?.SetProximity(0f);
            return;
        }

        var t = Mathf.InverseLerp(maxDistance, minDistance, dist);
        HapticManager.Instance?.SetProximity(Mathf.Lerp(hapticAtMaxDistance, hapticAtMinDistance, t));
    }

    void OnDisable()
    {
        HapticManager.Instance?.SetProximity(0f);
    }
}