using UnityEngine;

public class MoveCar : MonoBehaviour
{
    public float maxSpeed = 5f;
    public float fuel = 100;
    [SerializeField] public Driver driver;

    private void Update()
    {
        if (fuel > 0)
        {
            Vector3 dPosition = driver.Move(maxSpeed);
            transform.position += dPosition * Time.deltaTime;
            fuel -= Time.deltaTime;
        
            if (fuel < 0)
                fuel = 0;
        }
    }
}

