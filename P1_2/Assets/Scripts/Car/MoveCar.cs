using UnityEngine;

public class MoveCar : MonoBehaviour
{
    public float maxSpeed = 5f;
    public float fuel = 100;
    [SerializeField] private Driver driver;

    private void Update()
    {
        if (fuel > 0)
        {
            Vector3 dPosition = driver.move(maxSpeed);
            dPosition.z *= Mathf.Cos(transform.eulerAngles.y * Mathf.Deg2Rad);
            transform.position += dPosition * Time.deltaTime;
            fuel -= Time.deltaTime;
            if (fuel < 0) fuel = 0;
        }
    }

}

