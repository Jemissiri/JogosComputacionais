using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Offset")]
    [SerializeField] private float height   = 1f;
    [SerializeField] private float distance = 10f;
    [SerializeField] private float pitch    = 45f;  // degrees down from horizontal

    [Header("Follow")]
    [SerializeField] private float smoothSpeed = 8f;

    private Vector3 _offset;

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

        Vector3 desired = target.position + _offset;
        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
    }

    private void ComputeOffset()
    {
        // Build offset from pitch and distance so the inspector values are intuitive
        float pitchRad = pitch * Mathf.Deg2Rad;
        _offset = new Vector3(0f, Mathf.Sin(pitchRad) * distance, -Mathf.Cos(pitchRad) * distance);
        _offset = _offset.normalized * distance + Vector3.up * height;

        transform.rotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void OnValidate() => ComputeOffset();
}
