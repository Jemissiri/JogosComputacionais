using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError("Rigidbody component not found on the player.");
    }

    void FixedUpdate()
    {
        Vector2 moveInput = Vector2.zero;
        
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) 
            moveInput.y = 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) 
            moveInput.y = -1f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) 
            moveInput.x = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)    
            moveInput.x = 1f;

        Vector3 moveFoward = transform.forward * moveInput.y * speed * Time.fixedDeltaTime;
        Vector3 moveRight = transform.right * moveInput.x * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveFoward + moveRight);


    }
}
