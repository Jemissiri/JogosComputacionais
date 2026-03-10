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
        // rotate(dx);
        return new Vector3(dx, 0, dz) * maxSpeed;
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
        float boxWidth = 400;
        float boxHeight = 340;
        float boxY = Screen.height - boxHeight - 10f;

        GUI.Label(new Rect(10, boxY, boxWidth, boxHeight),
            "Road texture: https://pt.pinterest.com/pin/6685099438055248/\n\n" +
            "Grass texture:\nhttps://www.freepik.com/free-vector/seamless-green-grass-pattern_13187581.htm#fromView=keyword&page=1&position=1&uuid=470d536d-ffd9-4451-a048-571fb14cbeb2&query=Grass+texture\n\n" +
            "Balls texture: https://www.oddballs.co.uk/cdn/shop/files/Oddballs-Bouncing-Ball---65mm_pic1_odd_n.jpg?v=1695819130\n\n" +
            "Pillars texture:\nhttps://www.freepik.com/free-photo/background-made-from-bricks_10980125.htm#fromView=keyword&page=1&position=0&uuid=e26910bb-4e29-4d96-9650-49b2fa30ca52&query=Bricks+texture\n\n" +
            "The vehicles were developed by Unity\nfor the Junior Programmer learning pathway.");
    }

}
