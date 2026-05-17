using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    public float maxSpeed = 8f;
    public float gravity = 9.81f;
    public float rotationSpeed = 15f;
    public float blinkDistance = 5f;
    public float blinkCooldown = 0.8f;

    private CharacterController controller;
    private Animator animator;
    private Vector2 moveInput;
    private float verticalSpeed;
    private Quaternion targetRotation;
    private float blinkCooldownTimer;

    readonly int m_HashForwardSpeed  = Animator.StringToHash("ForwardSpeed");
    readonly int m_HashLateralSpeed  = Animator.StringToHash("LateralSpeed");
    readonly int m_HashGrounded      = Animator.StringToHash("Grounded");
    readonly int m_HashInputDetected = Animator.StringToHash("InputDetected");
    readonly int m_HashAngleDeltaRad = Animator.StringToHash("AngleDeltaRad");
    readonly int m_HashMeleeAttack   = Animator.StringToHash("MeleeAttack");
    readonly int m_HashStateTime     = Animator.StringToHash("StateTime");

    private float previousYAngle;

    void Awake()
    {
        controller               = GetComponent<CharacterController>();
        animator                 = GetComponent<Animator>();
        animator.applyRootMotion = false;
        targetRotation           = transform.rotation;
        previousYAngle           = transform.eulerAngles.y;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        blinkCooldownTimer -= Time.deltaTime;
        HandleGravity();
        HandleMovement();
        HandleRotation();
        HandleAnimator();
        HandleAttack();
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

    void HandleAttack()
    {
        animator.SetFloat(m_HashStateTime, Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f));

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            animator.SetTrigger(m_HashMeleeAttack);

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && blinkCooldownTimer <= 0f)
        {
            Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y);
            Vector3 blinkDir = input.sqrMagnitude > 0.01f ? input.normalized : transform.forward;

            Vector3 startPos = transform.position;
            BlinkGhost.Spawn(transform);
            controller.enabled = false;
            transform.position += blinkDir * blinkDistance;
            controller.enabled = true;
            BlinkGhost.SpawnTrail(startPos, transform.position);

            blinkCooldownTimer = blinkCooldown;
        }
    }

    public void MeleeAttackStart(int throwing = 0) { }
    public void MeleeAttackEnd() { }

    void HandleAnimator()
    {
        bool hasInput = moveInput.sqrMagnitude > 0f;

        float forwardSpeed = 0f;
        float lateralSpeed = 0f;
        if (hasInput)
        {
            Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
            forwardSpeed = Vector3.Dot(transform.forward, moveDir) * maxSpeed;
            lateralSpeed = Vector3.Dot(transform.right,   moveDir) * maxSpeed;
        }

        float currentYAngle = transform.eulerAngles.y;
        float angleDeltaRad = Mathf.DeltaAngle(previousYAngle, currentYAngle) * Mathf.Deg2Rad;
        previousYAngle = currentYAngle;

        animator.SetFloat(m_HashForwardSpeed,  forwardSpeed,  0.1f, Time.deltaTime);
        animator.SetFloat(m_HashLateralSpeed,  lateralSpeed,  0.1f, Time.deltaTime);
        animator.SetFloat(m_HashAngleDeltaRad, angleDeltaRad, 0.1f, Time.deltaTime);
        animator.SetBool(m_HashGrounded,      controller.isGrounded);
        animator.SetBool(m_HashInputDetected, hasInput);
    }
}
