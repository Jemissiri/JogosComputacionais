using UnityEngine;

public class MoveCar : MonoBehaviour
{
    public float maxSpeed;
    public float fuel;
    [SerializeField]
    private Driver driver;
    private Rigidbody rb; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError("Rigidbody component not found on the player.");
    }

    //TODO
    private void awake(){

    }

    //TODO
    private void FixedUpdate(){
        rb.MovePosition(rb.position + driver.move(maxSpeed));
    }
}

