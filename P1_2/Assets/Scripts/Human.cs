using UnityEngine;

class Human : Driver
{
    public override Vector3 move(float maxSpeed) {

        //get data from inputs
        float dx = 0; //TODO
        float dz = 0; //TODO
        return new Vector3(dx, 0, 0) * maxSpeed;
    }
}
