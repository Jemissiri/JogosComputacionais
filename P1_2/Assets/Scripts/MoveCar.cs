using UnityEngine;

public class MoveCar : MonoBehaviour
{
    public float maxSpeed;
    public float fuel;
    [SerializeField]
    private Driver driver;
    private Rigidbody rb; 

    private void Awake(){
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError("Rigidbody component not found on the player.");
    }

    //TODO fuel consumption
    private void FixedUpdate(){
        rb.MovePosition(rb.position + driver.move(maxSpeed));
    }
}

