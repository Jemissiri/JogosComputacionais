using UnityEngine;

public class MoveCar : MonoBehaviour
{
    public float maxSpeed;
    public float fuel;
    public Driver driver;
    private Rigidbody rb; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError("Rigidbody component not found on the player.");
    }

    //TODO
    public void awake(){

    }

    //TODO
    public void FixedUpdate(){
        rb.MovePosition(rb.position + driver.move(maxSpeed));
    }
}

