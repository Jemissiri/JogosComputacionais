using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

class Human : Driver
{
    private Vector2 _deltaMove;
    private int hits;

    public override Vector3 Move(float maxSpeed) 
    {
        float dz = _deltaMove.y;
        float dx = _deltaMove.x;
        return new Vector3(dx, 0, dz) * maxSpeed;
    }

    public void OnMove(InputValue value) 
    {
        _deltaMove = value.Get<Vector2>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Transform root = collision.transform.root;

        bool collidedWithVehicle = root.GetComponent<MoveCar>() != null;
        bool collidedWithBall = root.name.StartsWith("Ball");

        if (collidedWithVehicle || collidedWithBall)
            hits++;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), "hits: " + hits);
    }

}
