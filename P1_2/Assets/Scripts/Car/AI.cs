using UnityEngine;

class AI : Driver
{
    public override Vector3 move(float maxSpeed) {
        return Vector3.forward * maxSpeed;
    }
}
