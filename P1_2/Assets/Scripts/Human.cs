using UnityEngine;

class Human : Driver
{
    public override Vector3 move(float maxSpeed) {

        //get data from inputs
        dx = 0; //TODO
        dz = 0; //TODO
        return new Vector3(dx, 0, dz) * maxSpeed;
    }
}
