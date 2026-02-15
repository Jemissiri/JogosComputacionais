using UnityEngine;

public class MoveCar : MonoBehaviour
{
    public float maxSpeed = 5f;
    public float fuel = 100;
    [SerializeField] private Driver driver;
    private Rigidbody rb; 

    private Vector3 currentVelocity = Vector3.zero;

    private void Awake(){
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError("Rigidbody component not found on the player.");
    }

    //TODO fuel consumption

    private void Update()
    {
        if (fuel > 0)
        {
            Vector3 dPosition = driver.move(maxSpeed);
            transform.position += dPosition * Time.deltaTime;
            if (dPosition.sqrMagnitude > 0) // basicamente ver se o vetor nao e (0,0,0)
            {
                fuel -= Time.deltaTime;
                if (fuel < 0) fuel = 0;
            }
        }
    }

    // private void FixedUpdate(){
    //     rb.MovePosition(rb.position + driver.move(maxSpeed));
    // }
}

