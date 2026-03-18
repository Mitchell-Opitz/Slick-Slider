using UnityEngine;
using UnityEngine.InputSystem;

public sealed class HapticManager : MonoBehaviour
{
    public static HapticManager Instance { get; private set; }

    [SerializeField, Range(0f, 1f)] float masterScale = 1f;

    float proximityMotor;
    float slideMotor;
    float deathMotor;
    float deathExpiresAt;

    [SerializeField] float deathDuration = 0.5f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        if (Time.time > deathExpiresAt)
            deathMotor = 0f;

        var total = Mathf.Clamp01((proximityMotor + slideMotor + deathMotor) * masterScale);
        Gamepad.current?.SetMotorSpeeds(total, total);
    }

    void OnDestroy()
    {
        Gamepad.current?.SetMotorSpeeds(0f, 0f);
    }

    public void SetSlide(float normalized)
    {
        slideMotor = Mathf.Clamp01(normalized);
    }

    public void ClearSlide()
    {
        slideMotor = 0f;
    }

    public void SetProximity(float normalized)
    {
        proximityMotor = Mathf.Clamp01(normalized);
    }

    public void TriggerDeath()
    {
        deathMotor = 1f;
        deathExpiresAt = Time.time + deathDuration;
    }
}