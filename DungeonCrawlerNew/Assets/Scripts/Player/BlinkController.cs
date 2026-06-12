using UnityEngine;
using UnityEngine.InputSystem;

public class BlinkController : MonoBehaviour
{
    [Header("Blink")]
    [SerializeField] private float blinkDistance = 5f;
    [SerializeField] private float cooldown = 1.5f;

    [Header("VFX")]
    [SerializeField] private GameObject blinkVFXPrefab;
    [SerializeField] private float vfxDuration = 0.5f;

    private CharacterController _cc;
    private float _lastBlinkTime = -99f;
    private bool _isBlinking = false;
    private RaycastHit[] _hitBuffer = new RaycastHit[16];

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    public void OnBlink(InputValue value)
    {
        if (!value.isPressed) return;
        TryBlink();
    }

    private void TryBlink()
    {
        if (Time.time - _lastBlinkTime < cooldown) return;
        if (_isBlinking) return;

        Vector3 blinkDir = GetBlinkDirection();
        if (blinkDir == Vector3.zero) blinkDir = transform.forward;

        Vector3 destination = GetBlinkDestination(transform.position, blinkDir);

        // Spawn VFX at origin
        SpawnVFX(transform.position, blinkDir);

        // Teleport
        _cc.enabled = false;
        transform.position = destination;
        _cc.enabled = true;

        // Spawn VFX at destination
        SpawnVFX(destination, blinkDir);

        _lastBlinkTime = Time.time;
    }

    private Vector3 GetBlinkDestination(Vector3 origin, Vector3 dir)
    {
        float skinWidth = 0.15f;

        // CapsuleCast is unreliable against concave MeshColliders (same issue as camera).
        // Cast 3 parallel rays (center + left/right at capsule radius) to approximate the capsule shape.
        Vector3 side = Vector3.Cross(dir, Vector3.up).normalized * _cc.radius;
        Vector3 center = origin + _cc.center;
        Vector3[] rayOrigins = { center, center + side, center - side };

        bool prevBackfaces = Physics.queriesHitBackfaces;
        Physics.queriesHitBackfaces = true;

        float nearest = float.MaxValue;
        foreach (var o in rayOrigins)
        {
            int count = Physics.RaycastNonAlloc(o, dir, _hitBuffer, blinkDistance,
                                                ~0, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < count; i++)
            {
                var h = _hitBuffer[i];
                if (h.distance <= 0f) continue;
                if (h.transform.IsChildOf(transform.root)) continue;
                if (h.distance < nearest) nearest = h.distance;
            }
        }

        Physics.queriesHitBackfaces = prevBackfaces;

        if (nearest < float.MaxValue)
            return origin + dir * Mathf.Max(nearest - skinWidth, 0f);

        return origin + dir * blinkDistance;
    }

    private Vector3 GetBlinkDirection()
    {
        // Blink in the WASD movement direction in world space
        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) input.y += 1f;
        if (Keyboard.current.sKey.isPressed) input.y -= 1f;
        if (Keyboard.current.aKey.isPressed) input.x -= 1f;
        if (Keyboard.current.dKey.isPressed) input.x += 1f;

        if (input == Vector2.zero) return Vector3.zero;

        Camera cam = Camera.main;
        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        return (camForward * input.y + camRight * input.x).normalized;
    }

    private void SpawnVFX(Vector3 position, Vector3 direction)
    {
        if (blinkVFXPrefab == null) return;
        Quaternion rot = direction != Vector3.zero ? Quaternion.LookRotation(direction) : Quaternion.identity;
        GameObject vfx = Instantiate(blinkVFXPrefab, position, rot);

        if (vfx.TryGetComponent(out ParticleSystem ps))
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = ps.main;
            main.duration = vfxDuration;
            ps.Play();
        }

        Destroy(vfx, vfxDuration + 1f);
    }
}