using UnityEngine;

public sealed class AutoDestroyParticles : MonoBehaviour
{
    ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (ps == null) return;
        if (!ps.IsAlive())
            Destroy(gameObject);
    }
}
