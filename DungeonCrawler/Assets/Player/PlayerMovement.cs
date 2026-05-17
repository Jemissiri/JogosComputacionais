using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    public float maxSpeed = 8f;
    public float gravity = 9.81f;
    public float rotationSpeed = 15f;

    private CharacterController controller;
    private Animator animator;
    private Vector2 moveInput;
    private float verticalSpeed;
    private Quaternion targetRotation;

    readonly int m_HashForwardSpeed  = Animator.StringToHash("ForwardSpeed");
    readonly int m_HashGrounded      = Animator.StringToHash("Grounded");
    readonly int m_HashInputDetected = Animator.StringToHash("InputDetected");
    readonly int m_HashAngleDeltaRad = Animator.StringToHash("AngleDeltaRad");

    private float previousYAngle;

    void Awake()
    {
        controller     = GetComponent<CharacterController>();
        animator       = GetComponent<Animator>();
        targetRotation = transform.rotation;
        previousYAngle = transform.eulerAngles.y;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        HandleGravity();
        HandleMovement();
        HandleRotation();
        HandleAnimator();
    }

    void HandleGravity()
    {
        if (controller.isGrounded)
            verticalSpeed = -1f;
        else
            verticalSpeed -= gravity * Time.deltaTime;
    }

    void HandleMovement()
    {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 velocity = move * maxSpeed;
        velocity.y = verticalSpeed;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleRotation()
    {
        Vector2 mouseScreenPos = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : Vector2.zero;

        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPosition = ray.GetPoint(distance);
            Vector3 direction = worldPosition - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.5f)
                targetRotation = Quaternion.LookRotation(direction);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void HandleAnimator()
    {
        bool hasInput = moveInput.sqrMagnitude > 0f;

        float forwardSpeed = 0f;
        if (hasInput)
        {
            Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
            forwardSpeed = Vector3.Dot(transform.forward, moveDir) * maxSpeed;
        }

        float currentYAngle = transform.eulerAngles.y;
        float angleDeltaRad = Mathf.DeltaAngle(previousYAngle, currentYAngle) * Mathf.Deg2Rad;
        previousYAngle = currentYAngle;

        animator.SetFloat(m_HashForwardSpeed,  forwardSpeed,  0.1f, Time.deltaTime);
        animator.SetFloat(m_HashAngleDeltaRad, angleDeltaRad, 0.1f, Time.deltaTime);
        animator.SetBool(m_HashGrounded,      controller.isGrounded);
        animator.SetBool(m_HashInputDetected, hasInput);
    }
}