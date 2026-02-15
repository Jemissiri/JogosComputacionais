using UnityEngine;
using UnityEngine.InputSystem;

class Human : Driver
{
    public override Vector3 move(float maxSpeed) {

        //get data from inputs
        float dx = 0;
        float dz = 0;
        
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) dx = -1;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) dx = 1;

            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) dz = 1;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) dz = -1;
        }

        return new Vector3(dx, 0, dz) * maxSpeed;
    }
}
