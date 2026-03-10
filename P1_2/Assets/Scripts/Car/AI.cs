using UnityEngine;

class AI : Driver
{
    public Sensor sensor;
    private Vector3 moveVector;

    void Start() {
        moveVector = Vector3.forward;
    }
    public override Vector3 move(float maxSpeed) {
        if (sensor.Detected) {
            moveVector = sensor.Target.transform.position - transform.position;
            moveVector.Normalize();
            moveVector.z = Mathf.Abs(moveVector.z);
            return moveVector * maxSpeed;
        }
        return moveVector * maxSpeed;
    }
}
