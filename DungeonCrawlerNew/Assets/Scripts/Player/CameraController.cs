using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Offset")]
    [SerializeField] private float height   = 1f;
    [SerializeField] private float distance = 10f;
    [SerializeField] private float pitch    = 45f;

    [Header("Follow")]
    [SerializeField] private float smoothSpeed = 8f;

    [Header("Collision")]
    [SerializeField] private float collisionOffset = 0.2f;
    [SerializeField] private LayerMask collisionMask = ~0;

    private Vector3     _offset;
    private RaycastHit[] _hitBuffer = new RaycastHit[16];

    private void Start()
    {
        if (target == null)
        {
            var player = GameObject.Find("Y Bot");
            if (player != null) target = player.transform;
        }

        ComputeOffset();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos  = target.position + _offset;
        Vector3 adjustedPos = GetObstructionAdjustedPosition(target.position, desiredPos);

        transform.position = Vector3.Lerp(transform.position, adjustedPos, smoothSpeed * Time.deltaTime);
    }

    private Vector3 GetObstructionAdjustedPosition(Vector3 from, Vector3 to)
    {
        Vector3 dir     = to - from;
        float   maxDist = dir.magnitude;
        Vector3 dirNorm = dir / maxDist;

        bool prevBackfaces = Physics.queriesHitBackfaces;
        Physics.queriesHitBackfaces = true;

        // Cast from desired camera position toward player — outside→inside is reliable
        // with concave MeshColliders; inside→outside (SphereCast) is not.
        int count = Physics.RaycastNonAlloc(to, -dirNorm, _hitBuffer, maxDist, collisionMask,
                                            QueryTriggerInteraction.Ignore);

        Physics.queriesHitBackfaces = prevBackfaces;

        float nearestFromCamera = float.MaxValue;
        bool  anyHit            = false;

        for (int i = 0; i < count; i++)
        {
            var h = _hitBuffer[i];
            // Skip anything on the player's hierarchy (CharacterController etc.)
            if (target != null && h.transform.IsChildOf(target)) continue;
            if (h.distance < nearestFromCamera)
            {
                nearestFromCamera = h.distance;
                anyHit = true;
            }
        }

        if (anyHit)
        {
            // distFromPlayer = how far the wall is from the player along dirNorm.
            // Camera should sit just outside the wall on the camera's side (+offset),
            // clamped so it never exceeds the desired position.
            float distFromPlayer = maxDist - nearestFromCamera;
            return from + dirNorm * Mathf.Clamp(distFromPlayer + collisionOffset, 0f, maxDist);
        }

        return to;
    }

    private void ComputeOffset()
    {
        float pitchRad = pitch * Mathf.Deg2Rad;
        _offset = new Vector3(0f, Mathf.Sin(pitchRad) * distance, -Mathf.Cos(pitchRad) * distance);
        _offset = _offset.normalized * distance + Vector3.up * height;

        transform.rotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void OnValidate() => ComputeOffset();
}
