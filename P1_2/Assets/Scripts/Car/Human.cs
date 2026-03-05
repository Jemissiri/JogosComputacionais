using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

class Human : Driver
{
    private Vector2 _deltaMove;
    public override Vector3 move(float maxSpeed) {
        //move forward
        float dz = _deltaMove.y;
        float dx = _deltaMove.x;
        rotate(dx);
        return new Vector3(dz, 0, dz) * maxSpeed;
    }
    public void OnMove(InputValue value) {
        _deltaMove = value.Get<Vector2>();
    }

    private void rotate(float dz) {
        if (Mathf.Approximately(dz, 0f)) return;
        transform.Rotate(0f, Mathf.Sign(dz), 0f, Space.Self);
    }
}
