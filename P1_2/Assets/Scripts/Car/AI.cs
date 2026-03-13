using UnityEngine;

class AI : Driver
{
    public Sensor sensor;
    private Vector3 moveVector;

    void Start() 
    {
        moveVector = Vector3.forward;
        moveVector.z *= Mathf.Cos(transform.eulerAngles.y * Mathf.Deg2Rad);
    }

    public override Vector3 Move(float maxSpeed)
    {
        if (sensor.Detected)
        {
            moveVector = sensor.Target.transform.position - transform.position;
            moveVector.Normalize();
        }

        return moveVector * maxSpeed;
    }
}
