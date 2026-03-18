using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(LineRenderer))]
public sealed class Laser : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] LayerMask worldCollisionMask;
    [SerializeField] LayerMask playerMask;
    [SerializeField] LayerMask destroyMask;
    [SerializeField] Vector3 startOffsetLocal = Vector3.zero;
    [SerializeField] float maxDistance = 1000f;

    [Header("Direction")]
    [SerializeField] bool useUpAxis = true;

    [Header("State")]
    [SerializeField] bool enabledLaser = true;

    [Header("FX")]
    [SerializeField] ParticleSystem destroyParticlesPrefab;
    [SerializeField] AudioClip destroySfx;
    [SerializeField] AudioClip playerDeathSfx;

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
        CheckDestroyables(endLocal);
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

    void DoDestroyFX(GameObject go, Vector2 hitPoint, bool isPlayer)
    {
        Color c = Color.white;
        var sr = go.GetComponent<SpriteRenderer>();
        if (sr != null) c = sr.color;

        var ps = Instantiate(destroyParticlesPrefab, hitPoint, Quaternion.identity);
        var main = ps.main;
        main.startColor = c;

        AudioSource.PlayClipAtPoint(isPlayer ? playerDeathSfx : destroySfx, Camera.main.transform.position);

        Destroy(go);
    }

    void CheckPlayerHit(Vector3 endLocal)
    {
        var originWorld = transform.position;
        var endWorld = transform.TransformPoint(endLocal);
        var dist = Vector2.Distance(originWorld, endWorld);

        var hit = Physics2D.Raycast(originWorld, dirWorld, dist, playerMask);
        if (hit.collider != null)
        {
            Debug.Log("Game Over");
            ScreenShake.Instance?.Shake(0.5f, 1.0f);
            HapticManager.Instance?.TriggerDeath();
            ScreenFlash.Instance?.Flash(GetComponent<LaserVisuals>().LaserColor);
            GameOverScreen.Instance.TriggerGameOver();
            DoDestroyFX(hit.collider.gameObject, hit.point, true);
            //GameEvents.OnPlayerDeath?.Invoke();
        }
    }

    void CheckDestroyables(Vector3 endLocal)
    {
        var originWorld = transform.position;
        var endWorld = transform.TransformPoint(endLocal);
        var dist = Vector2.Distance(originWorld, endWorld);

        var hit = Physics2D.Raycast(originWorld, dirWorld, dist, destroyMask);
        if (hit.collider == null) return;

        DoDestroyFX(hit.collider.gameObject, hit.point, false);
    }
}
