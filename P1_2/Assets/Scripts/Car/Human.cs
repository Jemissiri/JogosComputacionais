using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

class Human : Driver
{
    private Vector2 _deltaMove;
    private int hits;

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

    private void OnCollisionEnter(Collision collision)
    {
        Transform root = collision.transform.root;

        bool collidedWithVehicle = root.GetComponent<MoveCar>() != null;
        bool collidedWithBall = root.name.StartsWith("Ball");

        if (collidedWithVehicle || collidedWithBall)
        {
            hits++;
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), "hits: " + hits);
    }

}
