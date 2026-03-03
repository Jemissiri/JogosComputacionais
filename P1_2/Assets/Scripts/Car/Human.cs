using UnityEngine;
using UnityEngine.InputSystem;

class Human : Driver
{
    private Vector2 _deltaMove;
    public override Vector3 move(float maxSpeed) {
        //move forward
        float dz = _deltaMove.y;
        float dx = _deltaMove.x;
        return new Vector3(dx, 0, dz) * maxSpeed;
    }
    public void OnMove(InputValue value) {
        _deltaMove = value.Get<Vector2>();
    }
}
