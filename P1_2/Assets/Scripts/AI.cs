using UnityEngine;

class AI : Driver
{
    public override Vector3 move(float maxSpeed) {
        //move forward
        return transform.forward * maxSpeed;
    }
}
