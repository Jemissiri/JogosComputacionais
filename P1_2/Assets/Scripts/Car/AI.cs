using UnityEngine;

class AI : Driver
{
    public override Vector3 move(float maxSpeed) {
        //move forward
        float dz = 1;
        return  new Vector3(0, 0, dz) * maxSpeed;
    }
}
