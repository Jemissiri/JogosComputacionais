using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float animationDampTime = 0.1f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;

    private CharacterController _cc;
    private Animator _animator;
    private Camera _camera;

    private Vector2 _moveInput;
    private Vector3 _worldMove;
    private Vector3 _velocity;

    private static readonly int HashMoveX = Animator.StringToHash("MoveX");
    private static readonly int HashMoveZ = Animator.StringToHash("MoveZ");

    private void Awake()
    {
        _cc       = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _camera   = Camera.main;
    }

    public void OnMove(InputValue value) => _moveInput = value.Get<Vector2>();

    private void Update()
    {
        FaceMouseCursor();
        Move();
        UpdateAnimator();
    }

    private void FaceMouseCursor()
    {
        // Cast a ray from the camera through the mouse position onto the ground plane
        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane ground = new(Vector3.up, transform.position);

        if (ground.Raycast(ray, out float distance))
        {
            Vector3 lookTarget = ray.GetPoint(distance);
            Vector3 dir = lookTarget - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    private void Move()
    {
        Vector3 camForward = _camera.transform.forward;
        Vector3 camRight   = _camera.transform.right;
        camForward.y = 0f;
        camRight.y   = 0f;
        camForward.Normalize();
        camRight.Normalize();

        _worldMove = camForward * _moveInput.y + camRight * _moveInput.x;

        if (_cc.isGrounded && _velocity.y < 0f)
            _velocity.y = -2f;
        _velocity.y += gravity * Time.deltaTime;

        _cc.Move((_worldMove * moveSpeed + Vector3.up * _velocity.y) * Time.deltaTime);
    }

    private void UpdateAnimator()
    {
        // Camera-relative world move projected into player local space.
        // Because the player always faces the cursor, pressing any WASD key
        // produces genuine strafe / backward / diagonal blends depending on
        // where the cursor is relative to the movement direction.
        Vector3 local = transform.InverseTransformDirection(_worldMove);

        _animator.SetFloat(HashMoveX, local.x, animationDampTime, Time.deltaTime);
        _animator.SetFloat(HashMoveZ, local.z, animationDampTime, Time.deltaTime);
    }
}
